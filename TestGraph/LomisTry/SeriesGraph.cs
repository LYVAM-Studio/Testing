namespace TestGraph.LomisTry;

public class SeriesGraph: Graph
{
    public SeriesGraph(Node entryPoint, Node exitPoint, List<Graph> subGraphs) : base(entryPoint, exitPoint, subGraphs)
    {
    }

    public override double ComputeEqResistance()
    {
        double eqR = 0d;
        foreach (var sub in SubGraphs)
            eqR += sub.EqResistance;
        return eqR;
    }

    public override void LaunchElectrons(double u)
    {
        var i = u / EqResistance;
        foreach (var sub in SubGraphs)
            sub.LaunchElectrons(sub.EqResistance * i);
    }
}