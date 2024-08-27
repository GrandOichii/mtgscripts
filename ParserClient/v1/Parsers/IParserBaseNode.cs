

using Godot;

public interface IParserBaseNode {
    public GraphNode AsGraphNode();
    public int GetAvailablePort(int candidate);

    public void ConnectParent(GraphEdit graph, IParserBaseNode parent, int port) {
        var node = AsGraphNode();
        var parentNode = parent.AsGraphNode();
        graph.ConnectNode(parentNode.Name, parent.GetAvailablePort(port), node.Name, 0);
    }
}