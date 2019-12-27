using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjReader.Data.Elements {

    public class Face {

        private readonly List<FaceVertex> _vertices = new List<FaceVertex>();

        public void AddVertex(FaceVertex vertex) {
            _vertices.Add(vertex);
        }

        public FaceVertex this[int i] {
            get { return _vertices[i]; }
        }

        public int Count {
            get { return _vertices.Count; }
        }
    }
}
