using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathematicalEntities {

    public class Trianglei {

        public Trianglei(int x0, int y0, int x1, int y1, int x2, int y2) {

            this.x0 = x0;
            this.y0 = y0;
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }

        public Trianglei(Vec2f v0, Vec2f v1, Vec2f v2) {

            this.x0 = (int)v0.x;
            this.y0 = (int)v0.y;
            this.x1 = (int)v1.x;
            this.y1 = (int)v1.y;
            this.x2 = (int)v2.x;
            this.y2 = (int)v2.y;
        }

        public Trianglei() { }

        public int x0;
        public int y0;
        public int x1;
        public int y1;
        public int x2;
        public int y2;
    }
}
