using TestGraph.Components;

namespace Reconnect.Electronics.Graph;

public static class GraphUtils
{
    /// <summary>
    /// Determines the list of all parallel branches, grouped by branches in parallel with each other
    /// </summary>
    /// <param name="branches">The branches from which extract the parallel ones</param>
    /// <returns>The list of all parallel branches groups</returns>
    public static List<List<Branch>> GetParallelBranchGroups(List<Branch> branches)
    {
        var visited = new HashSet<Branch>(); // stores the already visited branches
        var groups = new List<List<Branch>>(); // stores the list of all parallel branches groups

        foreach (var branch in branches)
        {
            if (visited.Contains(branch)) continue; // ignore all the branches already processed by the rec function

            var group = BuildParallelBranchGroup(branch, branches, visited); // fetch all the branches in parallel, including this branch
            
            // only add if there are more than 1 branch, then they are in parallel
            if (group.Count > 1)
                groups.Add(group);
        }

        return groups;
    }

    /// <summary>
    /// Adds to the <paramref name="group"/> of <see cref="Branch"/> the branches in parallel with the given <paramref name="branch"/>
    /// </summary>
    /// <param name="branch">The branch taken as reference</param>
    /// <param name="branches">The branches to test parallelness</param>
    /// <param name="visited">A set keeping track of already visited branches</param>
    /// <returns>The group of branches in which are added the branches in parallel with branch</returns>
    private static List<Branch> BuildParallelBranchGroup(Branch branch, List<Branch> branches, HashSet<Branch> visited)
    {
        List<Branch> group = new List<Branch>();
        visited.Add(branch);
        group.Add(branch);

        // Iterate through branches and find parallel branches that are not already visited
        foreach (var other in branches)
        {
            if (!visited.Contains(other) && branch.AreParallelBranches(other))
            {
                visited.Add(other);
                group.Add(other);
            }
        }

        return group;
    }

    public static Branch? MergeBranchInSeries(Branch branch, Graph graph)
    {
        return MergeBranchInSeries(branch, graph.Branches);
    }

    public static Branch? MergeBranchInSeries(Branch branch, List<Branch> branches)
    {
        Branch? merged = null;
        int i = 0;
        while (i < branches.Count && merged == null)
        {
            Branch graphBranch = branches[i];
            
            if (graphBranch.Nodes.n1 == branch.Nodes.n1 && graphBranch.Nodes.n1.AdjacentCount() == 2)
            {
                graphBranch.Nodes.n1 = branch.Nodes.n2;
                graphBranch.AddVertice(branch.Components);
                branches.Remove(branch);
                merged = graphBranch;
            }
            else if (graphBranch.Nodes.n1 == branch.Nodes.n2 && graphBranch.Nodes.n1.AdjacentCount() == 2)
            {
                graphBranch.Nodes.n1 = branch.Nodes.n1;
                graphBranch.AddVertice(branch.Components);
                branches.Remove(branch);
                merged = graphBranch;
            }
            else if (graphBranch.Nodes.n2 == branch.Nodes.n1 && graphBranch.Nodes.n2.AdjacentCount() == 2)
            {
                graphBranch.Nodes.n2 = branch.Nodes.n2;
                graphBranch.AddVertice(branch.Components);
                branches.Remove(branch);
                merged = graphBranch;
            }
            else if (graphBranch.Nodes.n2 == branch.Nodes.n2 && graphBranch.Nodes.n2.AdjacentCount() == 2)
            {
                graphBranch.Nodes.n2 = branch.Nodes.n1;
                graphBranch.AddVertice(branch.Components);
                branches.Remove(branch);
                merged = graphBranch;
            }

            i++;
        }

        return merged;
    }
}