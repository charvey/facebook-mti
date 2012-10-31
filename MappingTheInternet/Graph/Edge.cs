
namespace MappingTheInternet.Graph
{
    public class Edge<TEdge>
    {
        public readonly TEdge Value;

        public Edge(TEdge value)
        {
            Value = value;
        }
    }
}
