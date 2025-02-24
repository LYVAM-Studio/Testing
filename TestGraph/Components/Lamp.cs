using System.ComponentModel;
using Reconnect.Electronics.Graph;

namespace TestGraph.Components;

public class Lamp : ElecComponent
{
    public bool LightOn { get; private set; }
    public Lamp(string name, int resistance) : base(name, resistance)
    {
        LightOn = false;
    }

    public Lamp(string name, List<Vertice> adjacentComponents, int resistance) : base(name, adjacentComponents, resistance)
    {
        LightOn = false;
    }
}