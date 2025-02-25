namespace TestGraph.LomisTry;

public class Node
{
    public int Id { get; }
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
}