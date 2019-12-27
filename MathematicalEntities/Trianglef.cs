using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathematicalEntities {

    public class Trianglef {

        public Trianglef(float x0, float y0, float z0, float x1, float y1, float z1, float x2, float y2, float z2) {

            this.v0 = new Vec3f(x0, y0, z0);
            this.v1 = new Vec3f(x1, y1, z1);
            this.v2 = new Vec3f(x2, y2, z2);
        }

        public Trianglef(float x0, float y0, float x1, float y1, float x2, float y2) {

            this.v0 = new Vec3f(x0, y0, 0.0f);
            this.v1 = new Vec3f(x1, y1, 0.0f);
            this.v2 = new Vec3f(x2, y2, 0.0f);
        }

        public Trianglef(Trianglef other) {

            this.v0 = new Vec3f(other.v0);
            this.v1 = new Vec3f(other.v1);
            this.v2 = new Vec3f(other.v2);
        }

        public Trianglef(Vec2f v0, Vec2f v1, Vec2f v2) {

            this.v0 = new Vec3f(v0);
            this.v1 = new Vec3f(v1);
            this.v2 = new Vec3f(v2);
        }

        public Trianglef(Vec3f v0, Vec3f v1, Vec3f v2) {

            this.v0 = new Vec3f(v0);
            this.v1 = new Vec3f(v1);
            this.v2 = new Vec3f(v2);
        }

        public Trianglef(Vec4f v0, Vec4f v1, Vec4f v2) {

            this.v0 = new Vec3f(v0);
            this.v1 = new Vec3f(v1);
            this.v2 = new Vec3f(v2);
        }

        public Trianglef() {
            this.v0 = new Vec3f();
            this.v1 = new Vec3f();
            this.v2 = new Vec3f();
        }

        public Vec3f v0;
        public Vec3f v1;
        public Vec3f v2;

        public int[] order = new int[3] { 0, 1, 2 };
        public bool sorted = false;

        public float x0 { get => v0.x; set => v0.x = value; }
        public float y0 { get => v0.y; set => v0.y = value; }
        public float z0 { get => v0.z; set => v0.z = value; }

        public float x1 { get => v1.x; set => v1.x = value; }
        public float y1 { get => v1.y; set => v1.y = value; }
        public float z1 { get => v1.z; set => v1.z = value; }

        public float x2 { get => v2.x; set => v2.x = value; }
        public float y2 { get => v2.y; set => v2.y = value; }
        public float z2 { get => v2.z; set => v2.z = value; }

        public float r0 { get => v0.x; set => v0.x = value; }
        public float g0 { get => v0.y; set => v0.y = value; }
        public float b0 { get => v0.z; set => v0.z = value; }

        public float r1 { get => v1.x; set => v1.x = value; }
        public float g1 { get => v1.y; set => v1.y = value; }
        public float b1 { get => v1.z; set => v1.z = value; }

        public float r2 { get => v2.x; set => v2.x = value; }
        public float g2 { get => v2.y; set => v2.y = value; }
        public float b2 { get => v2.z; set => v2.z = value; }

        public Trianglef reClock() {

            return new Trianglef(v0, v2, v1);
        }

        public bool clockWise() {

            if (sorted) {
                return edgeFunction(v0, v2, v1) < 0.0f ? true : false;
            }
            else {
                Trianglef temp = reSort();
                return edgeFunction(temp.v0, temp.v2, temp.v1) < 0.0f ? true : false;
            }
        }

        public bool isEpsilonGeometry() {
            if ((GeneralVariables.FCMP(v0.x, v1.x) && GeneralVariables.FCMP(v1.x, v2.x)) || (GeneralVariables.FCMP(v0.y, v1.y) && GeneralVariables.FCMP(v1.y, v2.y)))
                return true;
            else
                return false;
        }

        public bool isOffScreen(Vec2f leftTop, Vec2f rightBottom) {
            if (sorted) {
                if (v2.y < leftTop.y || v0.y > rightBottom.y || (v0.x < leftTop.x && v1.x < leftTop.x && v2.x < leftTop.x) || (v0.x > rightBottom.x && v1.x > rightBottom.x && v2.x > rightBottom.x))
                    return true;
                else
                    return false;
            }
            else {
                Trianglef temp = reSort();
                if (temp.v2.y < leftTop.y || temp.v0.y > rightBottom.y || (temp.v0.x < leftTop.x && temp.v1.x < leftTop.x && temp.v2.x < leftTop.x) || (temp.v0.x > rightBottom.x && temp.v1.x > rightBottom.x && temp.v2.x > rightBottom.x))
                    return true;
                else
                    return false;
            }
        }

        public bool isFlatTop() {
            if (GeneralVariables.FCMP(v0.y, v1.y))
                return true;
            else {
                return false;
            }
        }

        public bool isFlatBottom() {
            if (GeneralVariables.FCMP(v1.y, v2.y))
                return true;
            else {
                return false;
            }
        }

        public Vec2f flatCoeff() {
            if (sorted) {
                float new_x = v0.x + (v1.y - v0.y) * (v2.x - v0.x) / (v2.y - v0.y);
                return new Vec2f(new_x, v1.y);
            }
            else {
                Trianglef temp = reSort();
                float new_x = temp.v0.x + (temp.v1.y - temp.v0.y) * (temp.v2.x - temp.v0.x) / (temp.v2.y - temp.v0.y);
                return new Vec2f(new_x, temp.v1.y);
            }
        }

        public Trianglef getBottomFlat() {
            if (sorted) {
                Vec2f flatEdge = flatCoeff();
                bool clockWise = this.clockWise();
                Vec3f w = clockWise ? this.nlambdas(flatEdge) : this.nlambdasCCW(flatEdge);
                return new Trianglef(
                    new Vec3f(v0),
                    new Vec3f(flatEdge, v0.z * w.x + v1.z * w.y + v2.z * w.z),
                    new Vec3f(v1)
                );
            }
            else {
                Trianglef temp = reSort();
                Vec2f flatEdge = temp.flatCoeff();
                bool clockWise = this.clockWise();
                Vec3f w = clockWise ? this.nlambdas(flatEdge) : this.nlambdasCCW(flatEdge);
                return new Trianglef(
                    new Vec3f(temp.v0),
                    new Vec3f(flatEdge, temp.v0.z * w.x + temp.v1.z * w.y + temp.v2.z * w.z),
                    new Vec3f(temp.v1)
                );
            }
        }

        public Trianglef getBottomFlat(Vec2f flatEdge) {
            if (sorted) {
                bool clockWise = this.clockWise();
                Vec3f w = clockWise ? this.nlambdas(flatEdge) : this.nlambdasCCW(flatEdge);
                return new Trianglef(
                    new Vec3f(v0),
                    new Vec3f(flatEdge, v0.z * w.x + v1.z * w.y + v2.z * w.z),
                    new Vec3f(v1)
                );
            }
            else {
                Trianglef temp = reSort();
                bool clockWise = this.clockWise();
                Vec3f w = clockWise ? this.nlambdas(flatEdge) : this.nlambdasCCW(flatEdge);
                return new Trianglef(
                    new Vec3f(temp.v0),
                    new Vec3f(flatEdge, temp.v0.z * w.x + temp.v1.z * w.y + temp.v2.z * w.z),
                    new Vec3f(temp.v1)
                );
            }
        }

        public Trianglef getTopFlat() {
            if (sorted) {
                Vec2f flatEdge = flatCoeff();
                bool clockWise = this.clockWise();
                Vec3f w = clockWise ? this.nlambdas(flatEdge) : this.nlambdasCCW(flatEdge);
                return new Trianglef(
                    new Vec3f(v1),
                    new Vec3f(flatEdge, v0.z * w.x + v1.z * w.y + v2.z * w.z),
                    new Vec3f(v2)
                );
            }
            else {
                Trianglef temp = reSort();
                Vec2f flatEdge = temp.flatCoeff();
                bool clockWise = this.clockWise();
                Vec3f w = clockWise ? this.nlambdas(flatEdge) : this.nlambdasCCW(flatEdge);
                return new Trianglef(
                    new Vec3f(temp.v1),
                    new Vec3f(flatEdge, temp.v0.z * w.x + temp.v1.z * w.y + temp.v2.z * w.z),
                    new Vec3f(temp.v2)
                );
            }
        }

        public Trianglef getTopFlat(Vec2f flatEdge) {
            if (sorted) {
                bool clockWise = this.clockWise();
                Vec3f w = clockWise ? this.nlambdas(flatEdge) : this.nlambdasCCW(flatEdge);
                return new Trianglef(
                    new Vec3f(v1),
                    new Vec3f(flatEdge, v0.z * w.x + v1.z * w.y + v2.z * w.z),
                    new Vec3f(v2)
                );
            }
            else {
                Trianglef temp = reSort();
                bool clockWise = this.clockWise();
                Vec3f w = clockWise ? this.nlambdas(flatEdge) : this.nlambdasCCW(flatEdge);
                return new Trianglef(
                    new Vec3f(temp.v1),
                    new Vec3f(flatEdge, temp.v0.z * w.x + temp.v1.z * w.y + temp.v2.z * w.z),
                    new Vec3f(temp.v2)
                );
            }
        }

        public Trianglef reSort() {
            Trianglef result = new Trianglef(this);
            result.sort();
            return result;
        }

        public void sort() {

            Vec3f A = v0;
            Vec3f B = v1;
            Vec3f C = v2;
            Vec3f Temp = new Vec3f();
            int t;

            if (B.y < A.y) {

                Temp = A;
                A = B;
                B = Temp;

                t = order[0];
                order[0] = order[1];
                order[1] = t;
            }

            if (C.y < A.y) {

                Temp = A;
                A = C;
                C = Temp;

                t = order[0];
                order[0] = order[2];
                order[2] = t;
            }

            if (A.y > C.y) {

                Temp = C;
                C = A;
                A = Temp;

                t = order[2];
                order[2] = order[0];
                order[0] = t;
            }

            if (B.y > C.y) {

                Temp = C;
                C = B;
                B = Temp;

                t = order[2];
                order[2] = order[1];
                order[1] = t;
            }

            v0 = A;
            v1 = B;
            v2 = C;
            sorted = true;
        }

        public float minX() {
            if (x0 < x1 && x0 < x2) return x0;
            if (x1 < x0 && x1 < x2) return x1;
            return x2;
        }

        public float maxX() {
            if (x0 > x1 && x0 > x2) return x0;
            if (x1 > x0 && x1 > x2) return x1;
            return x2;
        }

        public float minY() {
            if (y0 < y1 && y0 < y2) return y0;
            if (y1 < y0 && y1 < y2) return y1;
            return y2;
        }

        public float maxY() {
            if (y0 > y1 && y0 > y2) return y0;
            if (y1 > y0 && y1 > y2) return y1;
            return y2;
        }

        public static bool test(Vec3f w) {
            return test(w.x, w.y, w.z);
        }

        public static bool test(float w0, float w1, float w2) {

            if (w0 >= 0 && w1 >= 0 && w2 >= 0) {
                return true;
            }
            else {
                return false;
            }
        }

        public Vec3f center() {
            return new Vec3f(
                (v0.x + v1.x + v2.x) / 3.0f,
                (v0.y + v1.y + v2.y) / 3.0f,
                (v0.z + v1.z + v2.z) / 3.0f
            );
        }

        public bool test(Vec2f p) {

            Vec3f w = lambdas(p);
            float w0 = w.x;
            float w1 = w.y;
            float w2 = w.z;
            if (w0 >= 0 && w1 >= 0 && w2 >= 0) {
                return true;
            }
            else {
                return false;
            }
        }

        public Vec3f tangent() {

            return (v1 - v0).cross(v2 - v0);
        }

        public Vec3f normal() {

            return tangent().unit();
        }

        public Vec3f lambdas(Vec2f p) {

            /*
            float w0 = edgeFunction(v1, v2, new Vec3f(p));
            float w1 = edgeFunction(v2, v0, new Vec3f(p));
            float w2 = edgeFunction(v0, v1, new Vec3f(p));
            float w0 = edgeFunction(new Vec2f(v1), new Vec2f(v2), p);
            float w1 = edgeFunction(new Vec2f(v2), new Vec2f(v0), p);
            float w2 = edgeFunction(new Vec2f(v0), new Vec2f(v1), p);*/

            Vec2f v0 = new Vec2f(this.v0);
            Vec2f v1 = new Vec2f(this.v1);
            Vec2f v2 = new Vec2f(this.v2);

            float w0 = (p - v1).crossLenght(v2 - v1);
            float w1 = (p - v2).crossLenght(v0 - v2);
            float w2 = (p - v0).crossLenght(v1 - v0);

            return new Vec3f(w0, w1, w2);
        }

        public Vec3f lambdasCCW(Vec2f p) {

            Vec2f v0 = new Vec2f(this.v0);
            Vec2f v1 = new Vec2f(this.v1);
            Vec2f v2 = new Vec2f(this.v2);

            float w0 = (p - v2).crossLenght(v1 - v2);
            float w1 = (p - v1).crossLenght(v0 - v1);
            float w2 = (p - v0).crossLenght(v2 - v0);

            return new Vec3f(w0, w1, w2);
        }

        public Vec3f nlambdas(Vec2f p) {

            float area = this.area();
            float oneOverArea = 1.0f / area;
            //float w0 = edgeFunction(v1, v2, new Vec3f(p));
            //float w1 = edgeFunction(v2, v0, new Vec3f(p));
            //float w2 = edgeFunction(v0, v1, new Vec3f(p));

            Vec2f v0 = new Vec2f(this.v0);
            Vec2f v1 = new Vec2f(this.v1);
            Vec2f v2 = new Vec2f(this.v2);

            float w0 = (p - v1).crossLenght(v2 - v1);
            float w1 = (p - v2).crossLenght(v0 - v2);
            float w2 = (p - v0).crossLenght(v1 - v0);

            return new Vec3f(w0 * oneOverArea, w1 * oneOverArea, w2 * oneOverArea);
        }

        public Vec3f nlambdasCCW(Vec2f p) {

            float area = this.areaCCW();
            float oneOverArea = 1.0f / area;

            //Vec3f w = lambdasCCW(p);
            Vec2f v0 = new Vec2f(this.v0);
            Vec2f v1 = new Vec2f(this.v1);
            Vec2f v2 = new Vec2f(this.v2);

            float w0 = (p - v2).crossLenght(v1 - v2);
            float w1 = (p - v1).crossLenght(v0 - v1);
            float w2 = (p - v0).crossLenght(v2 - v0);

            return new Vec3f(w0 * oneOverArea, w2 * oneOverArea, w1 * oneOverArea);
        }

        public float area() {
            return edgeFunction(v0, v1, v2);
        }

        public float areaCCW() {
            return edgeFunction(v0, v2, v1);
        }

        public static float edgeFunction(Vec2f a, Vec2f b, Vec2f c) {
            //return (c[0] - a[0]) * (b[1] - a[1]) - (c[1] - a[1]) * (b[0] - a[0]);
            //return (c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x);
            return (c - a).crossLenght(b - a);
        }

        public static float edgeFunction(Vec3f a, Vec3f b, Vec3f c) {
            return edgeFunction(new Vec2f(a), new Vec2f(b), new Vec2f(c));
        }

        public Vec3f this[int key] {
            get {
                int i = order[key];
                Vec3f result;
                switch (i) {
                    case 0: {
                            result = v0;
                        }
                        break;
                    case 1: {
                            result = v1;
                        }
                        break;
                    case 2: {
                            result = v2;
                        }
                        break;
                    default: {
                            result = v0;
                        }
                        break;
                }
                return result;
            }
            set {
                int i = order[key];
                switch (i) {
                    case 0: {
                            v0 = value;
                        }
                        break;
                    case 1: {
                            v1 = value;
                        }
                        break;
                    case 2: {
                            v2 = value;
                        }
                        break;
                    default: {
                            v0 = value;
                        }
                        break;
                }
            }
        }
    }
}
