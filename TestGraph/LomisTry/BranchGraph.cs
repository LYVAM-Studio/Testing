namespace TestGraph.LomisTry;

public class BranchGraph : Graph
{
    public List<IElecComponent> Components { get; }
    public BranchGraph(Node entryPoint, Node exitPoint, List<IElecComponent> components) : base(entryPoint, exitPoint, new List<Graph>())
    {
        Components = components;
    }

    public override double ComputeEqResistance()
    {
        double eqR = 0d;
        foreach (var component in Components)
            eqR += component.R;
        return eqR;
    }

    public override void LaunchEletrons(double u, double i)
    {
        foreach (var component in Components)
        {
            component.DoSomethingWithCurrent(u, i);
        }
    }
}