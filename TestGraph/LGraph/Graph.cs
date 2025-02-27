namespace TestGraph.LGraph;

// The base class of the three types of graphs: SeriesGraph, ParallelGraph and BranchGraph.
public abstract class Graph
{
    public Node EntryPoint { get; }
    public Node ExitPoint { get; }
    protected List<Graph> SubGraphs { get; }
    private double _eqResistance;
    public double EqResistance
    {
        get
        {
            if (_eqResistance < 0)
                _eqResistance = ComputeEqResistance();

            return _eqResistance;
        }
    }

    protected Graph(Node entryPoint, Node exitPoint, List<Graph> subGraphs)
    {
        EntryPoint = entryPoint;
        ExitPoint = exitPoint;
        SubGraphs = subGraphs;
        _eqResistance = -1;
    }

    // Computes the equivalent resistance of this graph. Every subclass implements it own way of calculation r_eq.
    public abstract double ComputeEqResistance();
    // Simulated current of tension u in Volts flowing through this graph.
    public abstract void LaunchElectrons(double u);
}