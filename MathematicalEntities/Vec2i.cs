using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathematicalEntities {

    public class Vec2i {

        public int x;
        public int y;

        public Vec2i() {
            this.x = 0;
            this.y = 0;
        }

        public Vec2i(Vec2i other) {
            this.x = other.x;
            this.y = other.y;
        }

        public Vec2i(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public Vec2i(int value) {
            this.x = value;
            this.y = value;
        }

        public static Vec2i fromCeil(float x, float y) {
            return new Vec2i((int)Math.Ceiling(x), (int)Math.Ceiling(y));
        }

        public static Vec2i fromCeil(Vec2f other) {
            return new Vec2i((int)Math.Ceiling(other.x), (int)Math.Ceiling(other.y));
        }

        public static Vec2i fromCeil(Vec3f other) {
            return new Vec2i((int)Math.Ceiling(other.x), (int)Math.Ceiling(other.y));
        }

        public static Vec2i fromCeil(Vec4f other) {
            return new Vec2i((int)Math.Ceiling(other.x), (int)Math.Ceiling(other.y));
        }

        public static Vec2i fromRound(float x, float y) {
            return new Vec2i((int)Math.Round(x), (int)Math.Round(y));
        }

        public static Vec2i fromRound(Vec2f other) {
            return new Vec2i((int)Math.Round(other.x), (int)Math.Round(other.y));
        }

        public static Vec2i fromRound(Vec3f other) {
            return new Vec2i((int)Math.Round(other.x), (int)Math.Round(other.y));
        }

        public static Vec2i fromRound(Vec4f other) {
            return new Vec2i((int)Math.Round(other.x), (int)Math.Round(other.y));
        }

        public static Vec2i fromCut(float x, float y) {
            return new Vec2i((int)x, (int)y);
        }

        public static Vec2i fromCut(Vec2f other) {
            return new Vec2i((int)other.x, (int)other.y);
        }

        public static Vec2i fromCut(Vec3f other) {
            return new Vec2i((int)other.x, (int)other.y);
        }

        public static Vec2i fromCut(Vec4f other) {
            return new Vec2i((int)other.x, (int)other.y);
        }
    }
}
