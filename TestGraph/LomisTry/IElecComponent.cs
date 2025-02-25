namespace TestGraph.LomisTry;

public interface IElecComponent
{
    public string Name { get; }
    public double R { get; }
    public void DoSomethingWithCurrent(double i, double u)
    {
        Console.WriteLine($"{Name} received {u:F} V and {i:F} A");
        // Console.WriteLine($"{Name} received {u} V and {i} A");
    }
}

public class Resistor : IElecComponent
{
    public string Name { get; }
    public double R { get; }
    public Resistor(string name, double r)
    {
        Name = name;
        R = r;
    }
}

public class Light : IElecComponent
{
    public string Name { get; }
    public double R { get; }
    public Light(string name, double r)
    {
        Name = name;
        R = r;
    }
}
