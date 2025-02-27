namespace TestGraph.LGraph;

// The base class of all dipoles. 
public abstract class Component
{
    // The name of this component. Purely for visualisation.
    public string Name { get; set;}
    // The value of the resistance of this component.
    public double R { get; protected set;}
    // The tension U in Volts flowing through this dipole the last time it was plugged-in. By default, it is set to -1.
    public double ReceivedU { get; protected set;}
    // The intensity I in Amp flowing through this dipole the last time it was plugged-in. By default, it is set to -1.
    public double ReceivedI { get; protected  set; }
    // The action of this component when it is crossed by a current of tension u and intensity i.
    public void DoSomethingWithCurrent(double u, double i)
    {
        ReceivedU = u;
        ReceivedI = i;
        // Console.WriteLine($"{Name} received {u:F} V and {i:F} A");
    }
}

public class Resistor : Component
{
    public Resistor(string name, double r)
    {
        Name = name;
        R = r;
        ReceivedU = -1;
        ReceivedI = -1;
    }
}

public class Light : Component
{
    public Light(string name, double r)
    {
        Name = name;
        R = r;
        ReceivedU = -1;
        ReceivedI = -1;
    }
}
