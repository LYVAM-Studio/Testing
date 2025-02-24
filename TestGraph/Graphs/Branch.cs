using System.Collections.Generic;
using System.ComponentModel;
using TestGraph.Components;

namespace Reconnect.Electronics.Graph
{
    public class Branch
    {
        public List<Vertice> Components;
        public (Node n1, Node n2) Nodes;
        public int Resistance;

        public Branch(Node node1, Node node2)
        {
            Nodes.n1 = node1;
            Nodes.n2 = node2;
            Components = new List<Vertice>();
            Resistance = 0;
        }
        public Branch(Node node1, Node node2, IEnumerable<Vertice> components)
        {
            Nodes.n1 = node1;
            Nodes.n2 = node2;
            Components = new List<Vertice>();
            Resistance = 0;
            AddVertice(components);
        }

        public void AddVertice(Vertice vertice)
        {
            // skip if it is already in the list
            if (Components.Contains(vertice))
                return;
            
            Components.Add(vertice);
            if (vertice is ElecComponent component)
                Resistance += component.Resistance;
        }
        public void AddVertice(IEnumerable<Vertice> vertices)
        {
            foreach (Vertice vertice in vertices)
            {
                AddVertice(vertice);
            }
        }

        public bool AreParallelBranches(Branch other) =>
            (other.Nodes.n1, other.Nodes.n2) == (Nodes.n2, Nodes.n1) || other.Nodes == Nodes;
        
        public override string ToString() => $"{Nodes.n1} [{String.Join(", ", Components)}] {Nodes.n2}";
        public string Display() => $"{Nodes.n1} [{String.Join(", ", Components)}] {Nodes.n2} - Resistance : {Resistance} Ohms";
    }
}