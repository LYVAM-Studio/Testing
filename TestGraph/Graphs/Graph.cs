using System.Collections.Generic;
using System.Linq;
using TestGraph.Components;

namespace Reconnect.Electronics.Graph
{
    public class Graph
    {
        public string Name;
        public List<Vertice> Vertices;
        public List<Branch> Branches;
        public CircuitInput EntryPoint;
        public CircuitOutput ExitPoint;

        public Graph(string name, CircuitInput entryPoint, CircuitOutput exitPoint)
        {
            Name = name;
            Vertices = new List<Vertice>() ;
            Branches = new List<Branch>() ;
            EntryPoint = entryPoint;
            ExitPoint = exitPoint;
        }
        
        public Graph(string name, CircuitInput entryPoint, CircuitOutput exitPoint, List<Vertice> vertices)
        {
            Name = name;
            Vertices = vertices;
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
        
        private void RemoveAdjacentFromAllBranches(List<Branch> branches, Branch branchToRemove)
        {
            foreach (Branch branch in branches)
            {
                RemoveAdjacentFromBranchComponents(branchToRemove, branch.Nodes);
            }
        }
        
        public double GetGlobalIntensity()
        {
            if (Branches.Count == 0)
                throw new ArgumentException("No branches have been initialized in this circuit");
            List<Branch> tmpBranchCopy = new List<Branch>(Branches);
            var parallelBranchesGroups = GraphUtils.GetParallelBranchGroups(tmpBranchCopy);
            while (parallelBranchesGroups.Count != 0)
            {
                foreach (List<Branch> parallelBranches in parallelBranchesGroups)
                {
                    // int resistance = parallelBranches[0].Resistance;
                    double resistance = 0;
                    Node node1 = new Node(parallelBranches[0].Nodes.n1);
                    Node node2 = new Node(parallelBranches[0].Nodes.n2);
                    // tmpBranchCopy.Remove(parallelBranches[0]);
                    // int i = 1;
                    string name = "R_eq";
                    foreach (Branch branch in parallelBranches)
                    {
                        // RemoveAdjacentFromBranchComponents(branch, node1);
                        // RemoveAdjacentFromBranchComponents(branch, node2);
                        name += $"_{branch.GetHashCode()}";
                        tmpBranchCopy.Remove(branch);
                        if (branch.Resistance > 0)
                            resistance += 1 / (double) branch.Resistance;
                    }
                    /*while (i < parallelBranches.Count)
                    {
                        RemoveAdjacentFromBranchComponents(parallelBranches[i], node1);
                        RemoveAdjacentFromBranchComponents(parallelBranches[i], node2);
                        name += $"_{parallelBranches[i].GetHashCode()}";
                        tmpBranchCopy.Remove(parallelBranches[i]);
                        resistance = (resistance * parallelBranches[i].Resistance) / (resistance + parallelBranches[i].Resistance);
                        i++;
                    }*/
                    // TODO : fix remove components from adjacents of nodes from branches merged
                    Branch b = new Branch(node1, node2,
                        new List<Vertice> { new ElecComponent(name, 1/resistance) });
                    tmpBranchCopy.Add(b);
                    RemoveAdjacentFromAllBranches(tmpBranchCopy, b);
                    GraphUtils.MergeBranchInSeries(b, tmpBranchCopy);
                }
                parallelBranchesGroups = GraphUtils.GetParallelBranchGroups(tmpBranchCopy);
            }

            double totalResistance = 0;
            foreach (Branch branch in tmpBranchCopy)
            {
                totalResistance += branch.Resistance;
            }

            if (totalResistance == 0)
                throw new ArgumentException("No resistance in the circuit, maybe shortcut ?");

            return EntryPoint.InputTension / totalResistance;
        }
    }
}