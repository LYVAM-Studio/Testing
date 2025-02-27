namespace TestGraph.LGraph;

// Idea: remove this class and make components be graphs.

// This graph subclass represents the simple structure of a branch, i.e. a sequence of dipole linked in line with wires.
// It is the end of the recursion of the graph recursive structure.
public class BranchGraph : Graph
{
    public List<Component> Components { get; }
    public BranchGraph(Node entryPoint, Node exitPoint, List<Component> components) : base(entryPoint, exitPoint, [])
    {
        Components = components;
    }

    public override double ComputeEqResistance()
    {
        // The equivalent resistance of a branch is equal to the sum of the resistance of its components.
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