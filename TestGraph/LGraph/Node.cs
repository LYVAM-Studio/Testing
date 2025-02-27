namespace TestGraph.LGraph;

// A int wrapper class to represent nodes. It is purely for visualisation purposes. 
public class Node
{
    // The id of the node. It is used to test if a node is equal to another or not. It is supposed to be unique.
    public int Id { get; }
    // The name of the node, purely for visualisation. It does not impact the comparison between nodes.
    public string Name { get; set; }
    public Node(int id, string name = "")
    {
        Id = id;
        if (name == "") name = $"{id}";
        Name = name;
    }
    
    public static bool operator ==(Node node1, Node node2) => node1.Id == node2.Id;
    public static bool operator !=(Node node1, Node node2) => node1.Id != node2.Id;
    protected bool Equals(Node other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Node)obj);
    }

    public override int GetHashCode()
    {
        return Id;
    }
    
    // A shortcut to easily create a node. The name and id are randomly chosen.
    public static Node NewNode()
    {
        var i = new Random().Next();
        return new Node(i, $"{i}");
    }
}