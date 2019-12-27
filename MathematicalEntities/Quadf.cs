using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathematicalEntities {

    public class Quadf {

        public Quadf(float x0, float y0, float z0, float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3) {

            this.v0 = new Vec3f(x0, y0, z0);
            this.v1 = new Vec3f(x1, y1, z1);
            this.v2 = new Vec3f(x2, y2, z2);
            this.v3 = new Vec3f(x3, y3, z3);
        }

        public Quadf(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3) {

            this.v0 = new Vec3f(x0, y0, 0.0f);
            this.v1 = new Vec3f(x1, y1, 0.0f);
            this.v2 = new Vec3f(x2, y2, 0.0f);
        }

        public Quadf(Quadf other) {

            this.v0 = new Vec3f(other.v0);
            this.v1 = new Vec3f(other.v1);
            this.v2 = new Vec3f(other.v2);
            this.v3 = new Vec3f(other.v3);
        }

        public Quadf(Vec2f v0, Vec2f v1, Vec2f v2, Vec2f v3) {

            this.v0 = new Vec3f(v0);
            this.v1 = new Vec3f(v1);
            this.v2 = new Vec3f(v2);
            this.v3 = new Vec3f(v3);
        }

        public Quadf(Vec3f v0, Vec3f v1, Vec3f v2, Vec3f v3) {

            this.v0 = new Vec3f(v0);
            this.v1 = new Vec3f(v1);
            this.v2 = new Vec3f(v2);
            this.v3 = new Vec3f(v3);
        }

        public Quadf(Vec4f v0, Vec4f v1, Vec4f v2, Vec4f v3) {

            this.v0 = new Vec3f(v0);
            this.v1 = new Vec3f(v1);
            this.v2 = new Vec3f(v2);
            this.v3 = new Vec3f(v3);
        }

        public Quadf(Trianglef t0, Trianglef t1) {

            this.v0 = new Vec3f(t0.v0);
            this.v1 = new Vec3f(t0.v1);
            this.v2 = new Vec3f(t0.v2);
            this.v3 = new Vec3f(t1.v2);
        }

        public Quadf() {

            this.v0 = new Vec3f();
            this.v1 = new Vec3f();
            this.v2 = new Vec3f();
        }

        public Trianglef toTriangle1() {
            return new Trianglef(v0, v1, v2);
        }

        public Trianglef toTriangle2() {
            return new Trianglef(v0, v2, v3);
        }

        public Vec3f v0;
        public Vec3f v1;
        public Vec3f v2;
        public Vec3f v3;

        public float x0 { get => v0.x; set => v0.x = value; }
        public float y0 { get => v0.y; set => v0.y = value; }
        public float z0 { get => v0.z; set => v0.z = value; }

        public float x1 { get => v1.x; set => v1.x = value; }
        public float y1 { get => v1.y; set => v1.y = value; }
        public float z1 { get => v1.z; set => v1.z = value; }

        public float x2 { get => v2.x; set => v2.x = value; }
        public float y2 { get => v2.y; set => v2.y = value; }
        public float z2 { get => v2.z; set => v2.z = value; }

        public float x3 { get => v3.x; set => v3.x = value; }
        public float y3 { get => v3.y; set => v3.y = value; }
        public float z3 { get => v3.z; set => v3.z = value; }
    }
}
