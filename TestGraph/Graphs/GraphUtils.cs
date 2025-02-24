namespace Reconnect.Electronics.Graph;

public static class GraphUtils
{
    public static List<List<Branch>> GetParallelBranchGroups(List<Branch> branches)
    {
        var visited = new HashSet<Branch>();
        var groups = new List<List<Branch>>();

        foreach (var branch in branches)
        {
            if (visited.Contains(branch)) continue;

            var group = new List<Branch>();
            DFS_GetParallelBranchGroups(branch, branches, visited, group);
            // only add if there are more than 1 branch, then they are in // else meaningless
            if (group.Count > 1)
                groups.Add(group);
        }

        return groups;
    }

    private static void DFS_GetParallelBranchGroups(Branch branch, List<Branch> branches, HashSet<Branch> visited, List<Branch> group)
    {
        visited.Add(branch);
        group.Add(branch);

        foreach (var other in branches)
        {
            if (!visited.Contains(other) && branch.AreParallelBranches(other))
            {
                DFS_GetParallelBranchGroups(other, branches, visited, group);
            }
        }
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
            
            if (graphBranch.Nodes.n1 == branch.Nodes.n1 && graphBranch.Nodes.n1.AdjacentComponents.Count == 2)
            {
                graphBranch.Nodes.n1 = branch.Nodes.n2;
                graphBranch.AddVertice(branch.Components);
                branches.Remove(branch);
                merged = graphBranch;
            }
            else if (graphBranch.Nodes.n1 == branch.Nodes.n2 && graphBranch.Nodes.n1.AdjacentComponents.Count == 2)
            {
                graphBranch.Nodes.n1 = branch.Nodes.n1;
                graphBranch.AddVertice(branch.Components);
                branches.Remove(branch);
                merged = graphBranch;
            }
            else if (graphBranch.Nodes.n2 == branch.Nodes.n1 && graphBranch.Nodes.n2.AdjacentComponents.Count == 2)
            {
                graphBranch.Nodes.n2 = branch.Nodes.n2;
                graphBranch.AddVertice(branch.Components);
                branches.Remove(branch);
                merged = graphBranch;
            }
            else if (graphBranch.Nodes.n2 == branch.Nodes.n2 && graphBranch.Nodes.n2.AdjacentComponents.Count == 2)
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