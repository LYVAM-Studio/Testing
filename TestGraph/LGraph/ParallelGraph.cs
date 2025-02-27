namespace TestGraph.LGraph;

// A Graph subclass representing branches or more generally graphs, that are connected to the same entry and exit points in a parallel structure.
public class ParallelGraph : Graph
{
    public ParallelGraph(Node entryPoint, Node exitPoint, List<Graph> subGraphs) : base(entryPoint, exitPoint, subGraphs)
    {
    }

    public override double ComputeEqResistance()
    {
        // The equivalent resistance components (or graphs) in parallel is the inverse of the sum of the inverse of the resistances.
        double invdEqR = 0d;
        foreach (var sub in SubGraphs)
            invdEqR += 1 / sub.EqResistance;
        return 1 / invdEqR;
    }

    public override void LaunchElectrons(double u)
    {
        foreach (var sub in SubGraphs)
            sub.LaunchElectrons(u);
    }
}