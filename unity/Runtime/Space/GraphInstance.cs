using NumSharp;
using Gymize.Protobuf;

namespace Gymize
{
    public class GraphInstance : IInstance
    {
        private NDArray m_Nodes;
        public NDArray Nodes
        {
            get { return m_Nodes; }
            set { m_Nodes = value; }
        }
        private NDArray m_Edges;
        public NDArray Edges
        {
            get { return m_Edges; }
            set { m_Edges = value; }
        }
        private NDArray m_EdgeLinks;
        public NDArray EdgeLinks
        {
            get { return m_EdgeLinks; }
            set { m_EdgeLinks = value; }
        }

        public GraphInstance()
        {
            m_Nodes = null;
            m_Edges = null;
            m_EdgeLinks = null;
        }
        public GraphInstance(NDArray nodes, NDArray edges, NDArray edgeLinks)
        {
            m_Nodes = nodes;
            m_Edges = edges;
            m_EdgeLinks = edgeLinks;
        }

        public InstanceProto ToProtobuf()
        {
            GraphProto graphProto = new GraphProto
            {
                Nodes = new Tensor(m_Nodes).ToProtobuf().Tensor,
                Edges = new Tensor(m_Edges).ToProtobuf().Tensor,
                EdgeLinks = new Tensor(m_EdgeLinks).ToProtobuf().Tensor
            };
            InstanceProto instance = new InstanceProto
            {
                Type = InstanceTypeProto.Graph,
                Graph = graphProto
            };
            return instance;
        }

        public static object ParseFrom(GraphProto graphProto)
        {
            // Convert to GraphInstance
            GraphInstance graph = new GraphInstance();
            graph.Nodes = (NDArray)Tensor.ParseFrom(graphProto.Nodes);
            graph.Edges = (NDArray)Tensor.ParseFrom(graphProto.Edges);
            graph.EdgeLinks = (NDArray)Tensor.ParseFrom(graphProto.EdgeLinks);
            return graph;
        }

        public override string ToString()
        {
            string output = "";
            output += $"Nodes:\n{m_Nodes?.ToString()}\n";
            output += $"Edges:\n{m_Edges?.ToString()}\n";
            output += $"EdgeLinks:\n{m_EdgeLinks?.ToString()}";
            return output;
        }
    }
}