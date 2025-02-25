namespace TestGraph.LomisTry;

public class ParallelGraph : Graph
{
    public ParallelGraph(Node entryPoint, Node exitPoint, List<Graph> subGraphs) : base(entryPoint, exitPoint, subGraphs)
    {
    }

    public override double ComputeEqResistance()
    {
        double eqR = 0d;
        foreach (var sub in SubGraphs)
            eqR += 1 / sub.EqResistance;
        return 1 / eqR;
    }

    public override void LaunchElectrons(double u)
    {
        foreach (var sub in SubGraphs)
            sub.LaunchElectrons(u);
    }
}