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

        /// <summary>
        /// Creates a new Graph that goes from a point to another,
        /// and aims to get the tension at the terminals of the  <paramref name="target"/> component
        /// </summary>
        /// <param name="name">The name given to the graph</param>
        /// <param name="entryPoint">The circuit input</param>
        /// <param name="exitPoint">The circuit exit</param>
        /// <param name="target">The target of the circuit future computations</param>
        public Graph(string name, CircuitInput entryPoint, CircuitOutput exitPoint, ElecComponent target)
        {
            Name = name;
            Vertices = new List<Vertice>() ;
            Branches = new List<Branch>() ;
            EntryPoint = entryPoint;
            ExitPoint = exitPoint;
            Target = target;
        }
        /// <summary>
        /// Creates a new Graph that goes from a point to another,
        /// and aims to get the tension at the terminals of the <paramref name="target"/> component
        /// </summary>
        /// <param name="name">The name given to the graph</param>
        /// <param name="entryPoint">The circuit input</param>
        /// <param name="exitPoint">The circuit exit</param>
        /// <param name="vertices">The list of vertices contained in the graph</param>
        /// <param name="target">The target of the circuit future computations</param>
        public Graph(string name, CircuitInput entryPoint, CircuitOutput exitPoint, List<Vertice> vertices, ElecComponent target)
        {
            Name = name;
            Vertices = vertices;
            Target = target;
            Branches = new List<Branch>() ;
            EntryPoint = entryPoint;
            ExitPoint = exitPoint;
        }
        /// <summary>
        /// Add a vertice to the graph
        /// </summary>
        /// <param name="vertice">The vertice to add</param>
        public void AddVertice(Vertice vertice) => Vertices.Add(vertice);
        
        /// <summary>
        /// Add multiple vertices to the graph
        /// </summary>
        /// <param name="verticesList">The vertices to add</param>
        public void AddVertice(List<Vertice> verticesList) => Vertices.AddRange(verticesList);
        
        /// <summary>
        /// Build the list of all paths existing in the graph going from <c>EntryPoint</c> to <c>ExitPoint</c>
        /// </summary>
        /// <returns>The list of all paths</returns>
        /// <seealso cref="EntryPoint"/> <seealso cref="ExitPoint"/>
        private List<List<Vertice>> DfsFindPaths()
        {
            List<List<Vertice>> paths = new List<List<Vertice>>(); // list of all the paths fron Entry to Exit points
            HashSet<Vertice> visited = new HashSet<Vertice>(); // stores the already visited vertices to avoid traveling same vertice twice
            DfsPathRecursive(EntryPoint, new List<Vertice>(), paths, visited);
            return paths;
        }

        /// <summary>
        /// Recursively build the list of all paths from the current node to ExitPoint node
        /// </summary>
        /// <param name="current">The origin node of all paths</param>
        /// <param name="path">Path accumulator, not intended to be anything else than an empty list</param>
        /// <param name="paths">The lish in which the paths will be added</param>
        /// <param name="visited">A hashset that will keep track of visited nodes during the traversal</param>
        private void DfsPathRecursive(Vertice current, List<Vertice> path, List<List<Vertice>> paths, HashSet<Vertice> visited)
        {
            path.Add(current);
            visited.Add(current);

            // any path stops at ExitPoint
            if (current == ExitPoint)
            {
                paths.Add(new List<Vertice>(path)); // Store a copy of the path
            }
            else
            {
                foreach (var neighbor in current.AdjacentComponents)
                {
                    if (!visited.Contains(neighbor))
                    {
                        DfsPathRecursive(neighbor, path, paths, visited); // recursively build path in DFS through each neighbor nodes
                    }
                }
            }

            // At the end of each recursive call, the nodes met in the previous DFS traversal would be marked as visited
            // preventing to build other existing paths going through the same node at some point
            visited.Remove(current);
            
            // Backtrack
            // After a recursive call ends, we move up in the stack of calls, and removing the last node permits
            // to keep track of the previous common path with all the neighbors we are exploring
            path.RemoveAt(path.Count - 1);
        }
        /// <summary>
        /// Build the list of branches from the list of paths in a circuit
        /// </summary>
        /// <param name="paths">A list of paths from a node to another</param>
        /// <returns>The list of Branch contained into the paths given as params</returns>
        /// <seealso cref="DfsFindPaths"/>
        public List<Branch> ExtractBranches(List<List<Vertice>> paths)
        {
            List<Branch> branches = new List<Branch>();

            foreach (var path in paths)
            {
                if (path.Count < 2)
                    throw new ArgumentException("Invalid path : a path has at least 2 vertices");
                
                List<Vertice> branchVertices = new List<Vertice>(); // stores the vertices of the branch
                Vertice startNode = path[0]; // first node of the path is the start node of the branch
                
                // iterate through all the others nodes
                for (int i = 1; i < path.Count; i++)
                {
                    if (path[i] is Node) // Found a Node (End of branch)
                    {
                        // Create a Branch object with StartNode, EndNode, and collected vertices
                        Branch newBranch = new Branch(
                            (Node)startNode, // Start node
                            (Node)path[i], // End node
                            new List<Vertice>(branchVertices) // List of vertices on the branch
                            );
                        // if this branch is not already inside the branches list we add it
                        // this can happen when multiple path go through the same branch at some point of their path
                        if(branches.All(b => b != newBranch))
                            branches.Add(newBranch);

                        // Reset for the next branch
                        startNode = path[i];
                        branchVertices = new List<Vertice>(); // Start new branch
                    }
                    else
                    {
                        branchVertices.Add(path[i]); // Add current vertice to the branch
                    }
                }
                // if all vertices from paths have been converted into branches, this list should be empty
                if (branchVertices.Count != 0)
                {
                    throw new ArgumentException(
                        "Invalid path : there must be a start and an end node to each path to be valid");
                }
            }

            return branches;
        }
        
        /// <summary>
        /// Initialize the list of <see cref="Branches"/> with the branches of this graph belonging to a path
        /// from <c>EntryPoint</c> to <c>ExitPoint</c>
        /// 
        /// </summary>
        public void DefineBranches()
        {
            List<List<Vertice>> allPaths = DfsFindPaths(); // get all the paths
            List<Branch> branches = ExtractBranches(allPaths); // get all the branches fron these paths
            Branches = branches; // save it into the graph property
        }

        /// <summary>
        /// Remove a branch from the list of <see cref="Branches"/>  of the graph
        /// </summary>
        /// <param name="b"></param>
        public void RemoveBranch(Branch b) => Branches.Remove(b);

        /// <summary>
        /// Remove any component the given branch from the <see cref="Node.AdjacentComponents"/> of the given nodes
        /// </summary>
        /// <param name="branch">The branch</param>
        /// <param name="nodes">A node 2-tuple</param>
        private void RemoveAdjacentFromBranchComponents(Branch branch, (Node, Node) nodes)
        {
            foreach (Vertice branchComponent in branch.Components)
            {
                // remove if exists the component from the nodes adjacents
                nodes.Item1.AdjacentComponents.Remove(branchComponent); 
                nodes.Item2.AdjacentComponents.Remove(branchComponent);
            }
        }
        /// <summary>
        /// Permits to get the Input intensity required by the circuit
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when <see cref="Branches"/> is empty</exception>
        /// <exception cref="UnreachableException">Thrown when <see cref="Branches"/> becomes empty at the end of the precess</exception>
        /// <exception cref="GraphException">Thrown when the equivalent resistance of the circuit is 0</exception>
        public double GetGlobalIntensity()
        {
            if (Branches.Count == 0)
                throw new ArgumentException("No branches have been initialized in this circuit");

            // get the list of all branches parallel with each other
            List<List<Branch>> parallelBranchesGroups = GraphUtils.GetParallelBranchGroups(Branches); 
            // proceeds to associate branches and their resistance in parallel until there are no more
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
                        // remove the components of the branch fron the adjacents of the nodes
                        RemoveAdjacentFromBranchComponents(branch, (node1, node2)); 
                        name += $"_{branch.GetHashCode()}"; // build unique new name for equivalent resistance
                        RemoveBranch(branch);
                        if (branch.Resistance > 0)
                            resistance += 1 / (double) branch.Resistance;
                    }

                    Vertice equivalentResistance = new Resistor(name, 1 / resistance); // resistor representing the equivalent resistance 
                    node1.AddAdjacent(equivalentResistance);
                    node2.AddAdjacent(equivalentResistance);
                    
                    // new branch, result of the merge
                    Branch b = new Branch(node1, node2,
                        new List<Vertice> { equivalentResistance });
                    // merge it with any other branch in series with it, if there is any
                    GraphUtils.MergeBranchInSeries(b, Branches);
                    Branches.Add(b); // add the new branch to the Branches list
                }
                // search for new parallel branches groups
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
                throw new GraphException("No resistance in the circuit, maybe shortcut or empty ?");
            
            // Ohm's law : I = U / R
            return EntryPoint.InputTension / totalResistance;
        }

        /// <summary>
        /// Give the potential difference at the terminals of the <see cref="Target"/> component
        /// </summary>
        /// <returns>The tension in Volts</returns>
        public double GetVoltageTarget() => Target.GetVoltage(GetGlobalIntensity());
    }
}