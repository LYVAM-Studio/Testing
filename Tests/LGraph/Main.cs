namespace Tests.LGraph;

using TestGraph.LGraph;

public class Main
{
    // The {_tolerance}th leftmost digits after the point have to be equal for two floating point numbers to be equal in the following tests. The value must be from 0 to 15 included.
    private const int Tolerance = 3;

    private void Equality(double a, double b) => Assert.Equal(a, b, Tolerance);

    [Fact]
    public void SimpleSeriesGraph1()
    {
        var r1 = new Resistor("r1", 50);
        var g = new BranchGraph(Node.NewNode(), Node.NewNode(), [r1]);
        g.LaunchElectrons(230);
        Equality(230, r1.ReceivedU);
        Equality(4.6, r1.ReceivedI);
    }
    
    [Fact]
    public void SimpleSeriesGraph2()
    {
        var r1 = new Resistor("r1", 50);
        var r2 = new Resistor("r2", 50);
        var g = new BranchGraph(Node.NewNode(), Node.NewNode(), [r1, r2]);
        g.LaunchElectrons(230);
        Equality(115, r1.ReceivedU);
        Equality(2.3, r1.ReceivedI);
        Equality(115, r2.ReceivedU);
        Equality(2.3, r2.ReceivedI);
    }
    
    [Fact]
    public void SimpleSeriesGraph3()
    {
        var r1 = new Resistor("r1", 70);
        var r2 = new Resistor("r2", 22);
        var g = new BranchGraph(Node.NewNode(), Node.NewNode(), [r1, r2]);
        g.LaunchElectrons(230);
        Equality(175, r1.ReceivedU);
        Equality(2.5, r1.ReceivedI);
        Equality(55, r2.ReceivedU);
        Equality(2.5, r2.ReceivedI);
    }

    [Fact]
    public void SimpleParallelGraph1()
    {
        var n1 = Node.NewNode();
        var n2 = Node.NewNode();
        
        var r1 = new Resistor("r1", 50);
        var r2 = new Resistor("r2", 50);

        var b1 = new BranchGraph(n1, n2, [r1]);
        var b2 = new BranchGraph(n1, n2, [r2]);
        
        var g = new ParallelGraph(n1, n2, [b1, b2]);
        
        g.LaunchElectrons(230);
        
        Equality(230, r1.ReceivedU);
        Equality(4.6, r1.ReceivedI);
        Equality(230, r2.ReceivedU);
        Equality(4.6, r2.ReceivedI);
    }
    
    [Fact]
    public void SimpleParallelGraph2()
    {
        var n1 = Node.NewNode();
        var n2 = Node.NewNode();
        
        var r1 = new Resistor("r1", 74);
        var r2 = new Resistor("r2", 35);

        var b1 = new BranchGraph(n1, n2, [r1]);
        var b2 = new BranchGraph(n1, n2, [r2]);
        
        var g = new ParallelGraph(n1, n2, [b1, b2]);
        
        g.LaunchElectrons(230);
        
        Equality(230, r1.ReceivedU);
        Equality(3.108108108108, r1.ReceivedI);
        Equality(230, r2.ReceivedU);
        Equality(6.57142857142, r2.ReceivedI);
    }

    [Fact]
    public void ComplexGraph1()
    {
        var nodeIn = new Node(0, "IN");
        var nodeN1 = new Node(1, "N1");
        var nodeN2 = new Node(2, "N2");
        var nodeOut = new Node(3, "OUT");

        var l1 = new Light("L1", 529);
        var l2 = new Light("L2", 529);
        var r1 = new Resistor("R1", 200);
        var r2 = new Resistor("R2", 150);
        var r3 = new Resistor("R3", 100);
        var r4 = new Resistor("R4", 50);

        var branchInN1Left = new BranchGraph(nodeIn, nodeN1, [l1]);
        var branchInN1Right = new BranchGraph(nodeIn, nodeN1, [r2]);
        var branchN1N2 = new BranchGraph(nodeN1, nodeN2, [r4]);
        var branchInN2 = new BranchGraph(nodeIn, nodeN2, [r1, r3]);
        var branchN2Out = new BranchGraph(nodeN2, nodeOut, [l2]);

        var graphInN1 = new ParallelGraph(nodeIn, nodeN1, [branchInN1Left, branchInN1Right]);
        var graphInN2Right = new SeriesGraph(nodeIn, nodeN2, [graphInN1, branchN1N2]);
        var graphInN2 = new ParallelGraph(nodeIn, nodeN2, [branchInN2, graphInN2Right]);
        var main = new SeriesGraph(nodeIn, nodeOut, [graphInN2, branchN2Out]);

        main.LaunchElectrons(230);
        
        Equality(38.7623151182796765, l2.ReceivedU);
    }
}
