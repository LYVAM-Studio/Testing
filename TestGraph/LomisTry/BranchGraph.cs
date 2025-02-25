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

    public override void LaunchElectrons(double u)
    {
        var i = u / EqResistance;
        foreach (var component in Components)
            component.DoSomethingWithCurrent(component.R * i, i);
    }
}