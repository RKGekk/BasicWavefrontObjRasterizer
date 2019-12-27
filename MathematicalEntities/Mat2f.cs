using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathematicalEntities {

    public class Mat2f {

        public Vec2f[] data;

        public Mat2f() {

            data = new Vec2f[2];

            data[0] = new Vec2f(1.0f, 0.0f);
            data[1] = new Vec2f(0.0f, 1.0f);
        }

        public Mat2f(float s) {

            data = new Vec2f[2];

            data[0] = new Vec2f(s);
            data[1] = new Vec2f(s);
        }

        public Mat2f(Vec2f row1, Vec2f row2) {
            data = new Vec2f[2];

            data[0] = row1;
            data[1] = row2;
        }

        public Mat2f transpose() {

            Mat2f transpMat = new Mat2f();

            transpMat.data[0][0] = data[0][0];
            transpMat.data[0][1] = data[1][0];
            transpMat.data[1][0] = data[0][1];
            transpMat.data[1][1] = data[1][1];

            return transpMat;
        }

        public float determinant() {

            return data[0][0] * data[1][1] - data[0][1] * data[1][0];
        }

        public Mat2f inverse() {

            float oneOverDeterminant = 1.0f / determinant();
            return new Mat2f(new Vec2f(data[1][1] * oneOverDeterminant, -data[0][1] * oneOverDeterminant), new Vec2f(-data[1][0] * oneOverDeterminant, data[0][0] * oneOverDeterminant));
        }

        public float this[int i] {
            get {
                if (i == 0)
                    return this.data[0][0];
                if (i == 1)
                    return this.data[0][1];
                if (i == 2)
                    return this.data[1][0];
                return this.data[1][1];
            }
            set {
                if (i == 0)
                    this.data[0][0] = value;
                if (i == 1)
                    this.data[0][1] = value;
                if (i == 2)
                    this.data[1][0] = value;
                this.data[1][1] = value;
            }
        }

        public static Mat2f operator *(Mat2f m1, Mat2f m2) {

            return new Mat2f(
                new Vec2f(
                    m1.data[0][0] * m2.data[0][0] + m1.data[1][0] * m2.data[0][1],
                    m1.data[0][1] * m2.data[0][0] + m1.data[1][1] * m2.data[0][1]
                ),
                new Vec2f(
                    m1.data[0][0] * m2.data[1][0] + m1.data[1][0] * m2.data[1][1],
                    m1.data[0][1] * m2.data[1][0] + m1.data[1][1] * m2.data[1][1]
                )
            );
        }

        public static Vec2f operator *(Mat2f m1, Vec2f v1) {

            return new Vec2f(m1.data[0][0] * v1.x + m1.data[1][0] * v1.y, m1.data[0][1] * v1.x + m1.data[1][1] * v1.y);
        }

        public static Mat2f operator *(Mat2f m1, float s1) {
            return new Mat2f(
                new Vec2f(m1.data[0] * s1),
                new Vec2f(m1.data[1] * s1)
            );
        }

        public static Mat2f operator +(Mat2f m1, Mat2f m2) {

            Mat2f res = new Mat2f();
            res.data[0] = m1.data[0] + m2.data[0];
            res.data[1] = m1.data[1] + m2.data[1];

            return res;
        }
    }
}
