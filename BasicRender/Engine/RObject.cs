using MathematicalEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicRender.Engine {

    public class RObject {

        public int id;
        public string name;
        public int numPolygons;
        public float avgRadius;
        public float maxRadius;
        public Vec3f pos;
        public Mat4f world;
        public Mat4f model;
        public List<RPolygon> polygons;
    }
}
