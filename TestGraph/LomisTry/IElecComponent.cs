namespace TestGraph.LomisTry;



public interface IElecComponent
{
    public string Name { get; }
    public double R { get; }
    public void DoSomethingWithCurrent(double u, double i);
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

    public void DoSomethingWithCurrent(double u, double i)
    {
        Console.WriteLine($"R: {Name} received {u}V and {i}A");
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

    public void DoSomethingWithCurrent(double u, double i)
    {
        Console.WriteLine($"L: {Name} received {u}V and {i}A");
    }
}