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

        /*private void FillBranch(Branch branch, Vertice vertice, List<Vertice> alreadyVisited)
        {
            if (alreadyVisited.Contains(vertice))
                return;
            alreadyVisited.Add(vertice);
            if (vertice.Component is Node)
            {
                Console.WriteLine($"{vertice} is a node");
                branch.AddVertice(vertice); // add the last element of the branch
                Branches.Add(branch);
                
                foreach (Vertice adjacentComponent in vertice.AdjacentComponents)
                {
                    // ignore already visited adjacent components
                    if (alreadyVisited.Contains(adjacentComponent))
                        continue;
                    Console.WriteLine($"new branch created from {adjacentComponent}");
                    Branch newBranch = new Branch(); // new branch for each additional adjacent in this node
                    Branches.Add(newBranch);
                    newBranch.AddVertice(vertice); // add branch stating vertice
                    FillBranch(newBranch, adjacentComponent, alreadyVisited);
                }
            }
            else
            {
                Console.WriteLine($"{vertice} is a not a node");
                branch.AddVertice(vertice);
                foreach (Vertice adjacentComponent in vertice.AdjacentComponents)
                {
                    Console.WriteLine($"{adjacentComponent} added to the branch");
                    if (alreadyVisited.Contains(adjacentComponent))
                        continue;
                    FillBranch(branch, adjacentComponent, alreadyVisited);
                }
            }
        }*/

        private Branch BranchTraversalBuild(Vertice node, Vertice from, List<Vertice> alreadyVisited, out Vertice nextNode)
        {
            Branch branch = new Branch();
            branch.AddVertice(node);
            
            Vertice currentVertice = from;
            Vertice previousVertice = node;
            while (currentVertice.Component is not Node)
            {
                alreadyVisited.Add(currentVertice);
                Console.WriteLine($"{currentVertice}");
                branch.AddVertice(currentVertice);
                (currentVertice, previousVertice) = (currentVertice.AdjacentComponents.Find(v => v != previousVertice)!, currentVertice);
            }
            branch.AddVertice(currentVertice);
            nextNode = currentVertice;
            return branch;
        }
        
        private void FillBranch(Branch branch, Vertice vertice)
        {
            List<Vertice> alreadyVisited = new List<Vertice>();
            Queue<Vertice> nodesToVisit = new Queue<Vertice>();
            nodesToVisit.Enqueue(vertice);
            while (nodesToVisit.Count > 0)
            {
                Vertice node = nodesToVisit.Dequeue();
                Console.WriteLine($"{node} node visited");
                alreadyVisited.Add(node);
                foreach (Vertice verticeAdjacentComponent in node.AdjacentComponents)
                {
                    if (alreadyVisited.Contains(verticeAdjacentComponent))
                        continue;
                    Console.WriteLine($"branch traversal from {verticeAdjacentComponent}");
                    Branch b = BranchTraversalBuild(node, verticeAdjacentComponent,alreadyVisited, out Vertice nextNode);
                    Console.WriteLine($"{b}");
                    Branches.Add(b);
                    if (!alreadyVisited.Contains(nextNode))
                    {
                        Console.WriteLine($"{nextNode} is enqueued");
                        nodesToVisit.Enqueue(nextNode);
                    }
                }
            }
            
        }
        
        public void DefineBranches()
        {
            Branch branch = new Branch();
            FillBranch(branch, EntryPoint);
        }
    }
}