// See https://aka.ms/new-console-template for more information

using Reconnect.Electronics.Graph;
using TestGraph.Components;

Console.WriteLine("Hello, World!");

CircuitInput In = new CircuitInput("In", 230, 16);
CircuitOutput Out = new CircuitOutput("Out");

Graph g = new Graph("test", In, Out);
Vertice r1 = new ElecComponent("r1", 200);
Vertice r2 = new ElecComponent("r2", 150);
Vertice r3 = new ElecComponent("r3", 100);
Vertice l1 = new Lamp("l1", 529);
Vertice l2 = new Lamp("l2", 529);
Vertice s = new Switch("s", false);

Vertice n1 = new Node("n1");
Vertice n2 = new Node("n2");

List<Vertice> verticesList = new List<Vertice> {r1, r1, r3, l1, l2, s, n1, n2};
In.AddAdjacent(new List<Vertice>{r1, l1, r2});
Out.AddAdjacent(l2);
n1.AddAdjacent(new List<Vertice> {l1, r2, s});
n2.AddAdjacent(new List<Vertice> {r3, s, l2});

r1.AddAdjacent(new List<Vertice> {In, r3});
r2.AddAdjacent(new List<Vertice> {In, n1});
r3.AddAdjacent(new List<Vertice> {r1, n2});
l1.AddAdjacent(new List<Vertice> {In, n1});
l2.AddAdjacent(new List<Vertice> {n2, Out});
s.AddAdjacent(new List<Vertice> {n1, n2});
g.AddVertice(verticesList);
g.DefineBranches();

/*
Console.WriteLine('\n');
foreach (Branch branch in g.Branches)
{
    Console.WriteLine(branch);
}
*/

//var parallelBranchGroups = GraphUtils.GetParallelBranchGroups(g.Branches);

/*
foreach (List<Branch> parallelBranchGroup in parallelBranchGroups)
{
    string branches = String.Join(" | ", parallelBranchGroup);
    Console.WriteLine(branches);
}*/

double I = g.GetGlobalIntensity();
Console.WriteLine($"{I} Amps");

Console.WriteLine("breakpoint");