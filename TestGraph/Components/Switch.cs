using Reconnect.Electronics.Graph;

namespace TestGraph.Components;

public class Switch : ElecComponent
{
    public bool IsOpen { get; private set; }
    
    public Switch(string name, bool isOpen) : base(name, 0)
    {
        IsOpen = isOpen;
    }

    public Switch(string name, List<Vertice> adjacentComponents, bool isOpen) : base(name, adjacentComponents, 0)
    {
        IsOpen = isOpen;
    }

    public void Toggle() => IsOpen = !IsOpen;
}