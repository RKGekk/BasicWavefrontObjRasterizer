using MathematicalEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicRender.Engine {

    public class RVertex {

        public float x;
        public float y;
        public float z;

        public float r;
        public float g;
        public float b;

        public float nx;
        public float ny;
        public float nz;

        public float u0;
        public float v0;

        public RVertex(float x, float y, float z, float r, float g, float b, float nx, float ny, float nz, float u0, float v0) {

            this.x = x;
            this.y = y;
            this.z = z;
            
            this.r = r;
            this.g = g;
            this.b = b;
            
            this.nx = nx;
            this.ny = ny;
            this.nz = nz;
            
            this.u0 = u0;
            this.v0 = v0;
        }

        public RVertex(Vec3f pos, Vec3f color, Vec3f norm, Vec2f texPos) {

            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;

            this.r = color.x;
            this.g = color.y;
            this.b = color.z;

            this.nx = norm.x;
            this.ny = norm.y;
            this.nz = norm.z;

            this.u0 = texPos.x;
            this.v0 = texPos.y;
        }
    }
}
