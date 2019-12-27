using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjReader.Data.VertexData {

    public struct Vertex {

        public Vertex(float x, float y, float z) : this() {
            X = x;
            Y = y;
            Z = z;
        }

        public Vertex(float x, float y, float z, float r, float g, float b) : this() {
            X = x;
            Y = y;
            Z = z;

            R = r;
            G = g;
            B = b;
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public float R { get; private set; }
        public float G { get; private set; }
        public float B { get; private set; }
    }
}
