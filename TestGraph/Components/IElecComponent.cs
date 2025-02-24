using Reconnect.Electronics.Graph;

namespace TestGraph.Components;

public class ElecComponent : Vertice
{
    public int Resistance { get; set; }
    
    public ElecComponent(string name, int resistance) : base(name)
    {
        if (resistance < 0)
            throw new ArgumentException("Resistance of a component cannot be negative");
        Resistance = resistance;
    }
    public ElecComponent(string name, List<Vertice> adjacentComponents, int resistance) : base(name, adjacentComponents)
    {
        if (resistance < 0)
            throw new ArgumentException("Resistance of a component cannot be negative");
        Resistance = resistance;
    }
    
}