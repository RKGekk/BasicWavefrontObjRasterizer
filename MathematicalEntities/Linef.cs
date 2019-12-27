using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathematicalEntities {

    public class Linef {

        public Vec3f t0 { get; set; }
        public Vec3f t1 { get; set; }

        public float x0 { get => t0.x; set => t0.x = value; }
        public float y0 { get => t0.y; set => t0.y = value; }
        public float x1 { get => t1.x; set => t1.x = value; }
        public float y1 { get => t1.y; set => t1.y = value; }

        public Linef(float x0, float y0, float x1, float y1) {

            this.t0 = new Vec3f();
            this.t0.x = x0;
            this.t0.y = y0;

            this.t1 = new Vec3f();
            this.t1.x = x1;
            this.t1.y = y1;
        }

        public Linef(Vec4f t0, Vec4f t1) {

            this.t0 = new Vec3f(t0);
            this.t1 = new Vec3f(t1);
        }

        public Linef(Vec3f t0, Vec3f t1) {

            this.t0 = t0;
            this.t1 = t1;
        }

        public Linef(Vec2f t0, Vec2f t1) {

            this.t0 = new Vec3f(t0);
            this.t1 = new Vec3f(t1);
        }
    }
}
