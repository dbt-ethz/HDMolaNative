using System.Collections.Generic;

public class MolaDirectedGraph
{
    List<List<DirectedEdge>> edges;
    public MolaDirectedGraph(int nNodes)
    {
        InitList(nNodes);
    }
    private void InitList(int nNodes)
    {
        edges = new List<List<DirectedEdge>>(nNodes);
        for (int i = 0; i < nNodes; i++)
        {
            edges.Add(new List<DirectedEdge>());
        }
        
    }
    public void AddDirectedEdge(int node1,int node2,float weight=1)
    {
        List<DirectedEdge> nodeEdges = this.edges[node1];
        nodeEdges.Add(new DirectedEdge(node1, node2, weight));
    }
    public void AddDoubleEdge(int node1, int node2, float weight1 = 1,float weight2=1)
    {
        AddDirectedEdge(node1, node2, weight1);
        AddDirectedEdge(node2, node1, weight2);
    }
    public float SetDirectedWeight(int node1, int node2,float weight)
    {
        return 1;
    }
    public DirectedEdge GetDirectedEdge(int node1, int node2)
    {
        foreach (DirectedEdge edge in GetDirectedEdges(node1))
        {
            if (edge.n2 == node2) return edge;
        }
        return null;
    }
    public void SetWeight(int node1, int node2, float weight)
    {
        DirectedEdge edge = GetDirectedEdge(node1, node2);
        if (edge != null)
        {
            edge.weight = weight;
        }
    }
    public int NodesCount()
    {
        return edges.Count;
    }
    public List<DirectedEdge> GetDirectedEdges(int node)
    {
        return edges[node];
    }
    public class DirectedEdge
    {
        public DirectedEdge(int n1, int n2, float weight = 1)
        {
            this.n1 = n1;
            this.n2 = n2;
            this.weight = weight;
        }
        public int n1 = -1;
        public int n2 = -1;
        public float weight = 1;
    }
}
