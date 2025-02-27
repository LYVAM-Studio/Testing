using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TestGraph.Components;

namespace Reconnect.Electronics.Graph
{
    public class Graph
    {
        public string Name;
        public List<Vertice> Vertices;
        public List<Branch> Branches;
        public CircuitInput EntryPoint;
        public ElecComponent Target;
        public CircuitOutput ExitPoint;

        public Graph(string name, CircuitInput entryPoint, CircuitOutput exitPoint, ElecComponent target)
        {
            Name = name;
            Vertices = new List<Vertice>() ;
            Branches = new List<Branch>() ;
            EntryPoint = entryPoint;
            ExitPoint = exitPoint;
            Target = target;
        }
        
        public Graph(string name, CircuitInput entryPoint, CircuitOutput exitPoint, List<Vertice> vertices, ElecComponent target)
        {
            Name = name;
            Vertices = vertices;
            Target = target;
            Branches = new List<Branch>() ;
            EntryPoint = entryPoint;
            ExitPoint = exitPoint;
        }
        
        public void AddVertice(Vertice vertice) => Vertices.Add(vertice);
        public void AddVertice(List<Vertice> verticesList) => Vertices.AddRange(verticesList);

        private Branch BranchTraversalBuild(Vertice node, Vertice from, List<Vertice> alreadyVisited, out Vertice nextNode)
        {
            List<Vertice> branchVertices = new List<Vertice>();
            Vertice currentVertice = from;
            Vertice previousVertice = node;
            
            // branchVertices.Add(node);
            while (currentVertice is not Node)
            {
                alreadyVisited.Add(currentVertice);
                //Console.WriteLine($"{currentVertice}");
                branchVertices.Add(currentVertice);
                (currentVertice, previousVertice) = (currentVertice.AdjacentComponents.Find(v => v != previousVertice)!, currentVertice);
            }
            // branchVertices.Add(currentVertice);
            nextNode = currentVertice;
            return new Branch((Node)node, (Node)currentVertice, branchVertices);
        }
        
        private void FillBranch(Vertice vertice)
        {
            List<Vertice> alreadyVisited = new List<Vertice>();
            Queue<Vertice> nodesToVisit = new Queue<Vertice>();
            nodesToVisit.Enqueue(vertice);
            while (nodesToVisit.Count > 0)
            {
                Vertice node = nodesToVisit.Dequeue();
                //Console.WriteLine($"{node} node visited");
                alreadyVisited.Add(node);
                foreach (Vertice verticeAdjacentComponent in node.AdjacentComponents)
                {
                    if (alreadyVisited.Contains(verticeAdjacentComponent))
                        continue;
                    //Console.WriteLine($"branch traversal from {verticeAdjacentComponent}");
                    Branch b = BranchTraversalBuild(node, verticeAdjacentComponent,alreadyVisited, out Vertice nextNode);
                    //Console.WriteLine($"{b}");
                    Branches.Add(b);
                    if (!alreadyVisited.Contains(nextNode))
                    {
                        //Console.WriteLine($"{nextNode} is enqueued");
                        nodesToVisit.Enqueue(nextNode);
                    }
                }
            }
            
        }
        
        public void DefineBranches()
        {
            FillBranch(EntryPoint);
        }

        public void RemoveBranch(Branch b) => Branches.Remove(b);

        private void RemoveAdjacentFromBranchComponents(Branch branch, (Node, Node) nodes)
        {
            foreach (Vertice branchComponent in branch.Components)
            {
                nodes.Item1.AdjacentComponents.Remove(branchComponent);
                nodes.Item2.AdjacentComponents.Remove(branchComponent);
            }
        }
        
        public double GetGlobalIntensity()
        {
            if (Branches.Count == 0)
                throw new ArgumentException("No branches have been initialized in this circuit");

            var parallelBranchesGroups = GraphUtils.GetParallelBranchGroups(Branches);
            while (parallelBranchesGroups.Count != 0)
            {
                foreach (List<Branch> parallelBranches in parallelBranchesGroups)
                {
                    double resistance = 0;
                    Node node1 = parallelBranches[0].StartNode; // future nodes of the equivalent branch
                    Node node2 = parallelBranches[0].EndNode;
                    string name = "R_eq";
                    foreach (Branch branch in parallelBranches)
                    {
                        RemoveAdjacentFromBranchComponents(branch, (node1, node2));
                        name += $"_{branch.GetHashCode()}";
                        RemoveBranch(branch);
                        if (branch.Resistance > 0)
                            resistance += 1 / (double) branch.Resistance;
                    }

                    Vertice equivalentResistance = new Resistor(name, 1 / resistance); // resistor representing the equivalent resistance 
                    node1.AddAdjacent(equivalentResistance);
                    node2.AddAdjacent(equivalentResistance);
                    Branch b = new Branch(node1, node2,
                        new List<Vertice> { equivalentResistance });
                    Branches.Add(b);
                    GraphUtils.MergeBranchInSeries(b, Branches);
                }
                parallelBranchesGroups = GraphUtils.GetParallelBranchGroups(Branches);
            }

            if (Branches.Count > 1)
                throw new UnreachableException("There should be only one branch remaining");
            /*foreach (Branch branch in Branches)
            {
                totalResistance += branch.Resistance;
            }*/
            double totalResistance = Branches[0].Resistance;

            if (totalResistance == 0)
                throw new ArgumentException("No resistance in the circuit, maybe shortcut or empty ?");

            return EntryPoint.InputTension / totalResistance;
        }

        public double GetVoltageTarget() => Target.GetVoltage(GetGlobalIntensity());
    }
}