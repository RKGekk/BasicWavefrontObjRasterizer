using ObjReader.Common;
using ObjReader.Data.DataStore;
using ObjReader.Data.VertexData;
using ObjReader.TypeParsers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjReader.TypeParsers {

    public class VertexParser : TypeParserBase, IVertexParser {

        private readonly IVertexDataStore _vertexDataStore;

        public VertexParser(IVertexDataStore vertexDataStore) {
            _vertexDataStore = vertexDataStore;
        }

        protected override string Keyword {
            get { return "v"; }
        }

        public override void Parse(string line) {
            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var x = parts[0].ParseInvariantFloat();
            var y = parts[1].ParseInvariantFloat();
            var z = parts[2].ParseInvariantFloat();

            if (parts.Length == 6) {
                var r = parts[3].ParseInvariantFloat();
                var g = parts[4].ParseInvariantFloat();
                var b = parts[5].ParseInvariantFloat();
                var vertex = new Vertex(x, y, z, r, g, b);
                _vertexDataStore.AddVertex(vertex);
            }
            else {
                var vertex = new Vertex(x, y, z);
                _vertexDataStore.AddVertex(vertex);
            }
        }
    }
}
