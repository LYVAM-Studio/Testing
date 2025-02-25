namespace TestGraph.LomisTry;

public abstract class Graph
{
    public Node EntryPoint { get; }
    public Node ExitPoint { get; }
    public List<Graph> SubGraphs { get; }
    protected double eqResistance;
    public double EqResistance
    {
        get
        {
            if (eqResistance == -1d)
            {
                eqResistance = ComputeEqResistance();
            }

            return eqResistance;
        }
    }

    public Graph(Node entryPoint, Node exitPoint, List<Graph> subGraphs)
    {
        EntryPoint = entryPoint;
        ExitPoint = exitPoint;
        SubGraphs = subGraphs;
        eqResistance = -1;
    }

    public abstract double ComputeEqResistance();
    public abstract void LaunchElectrons(double u);
}