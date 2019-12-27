using MathematicalEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicRender.Engine {

    public class RPolygon {

        public enum RasterType : int {
            FlatColor,
            InterpolatedColor,
            Textured
        }

        public RPolygon() { }

        public RPolygon(RPolygon other) {

            this.geometry = new Trianglef(other.geometry);
            this.color = new Trianglef(other.color);
            this.textureCoords = new Trianglef(other.textureCoords);
        }

        public RPolygon(Trianglef geometry, Trianglef color,  Trianglef textureCoords) {

            this.geometry = geometry;
            this.color = color;
            this.textureCoords = textureCoords;
        }

        public RPolygon(Trianglef geometry, Trianglef color, Trianglef textureCoords, RasterType rasterType) {

            this.geometry = geometry;
            this.color = color;
            this.textureCoords = textureCoords;
            this.rasterType = rasterType;
        }

        public RasterType rasterType;
        public Trianglef geometry;
        public Trianglef color;
        public Trianglef textureCoords;

        public void calc() {

            this.center = geometry.center();
            this.normal = geometry.normal();
        }

        public Vec3f normal;
        public Vec3f center;

        public RPolygon reSort() {
            RPolygon result = new RPolygon(this);
            result.sort();
            return result;
        }

        public void sort() {

            this.geometry.sort();

            Trianglef temp = new Trianglef(color);

            color.v0 = temp[this.geometry.order[0]];
            color.v1 = temp[this.geometry.order[1]];
            color.v2 = temp[this.geometry.order[2]];

            temp = new Trianglef(textureCoords);

            textureCoords.v0 = temp[this.geometry.order[0]];
            textureCoords.v1 = temp[this.geometry.order[1]];
            textureCoords.v2 = temp[this.geometry.order[2]];
        }

        public RPolygon getBottomFlat() {

            RPolygon result = new RPolygon();

            if (this.geometry.sorted) {

                Vec2f flatEdge = this.geometry.flatCoeff();
                result.geometry = this.geometry.getBottomFlat(flatEdge);

                bool clockWise = this.geometry.clockWise();
                Vec3f w = clockWise ? this.geometry.nlambdas(flatEdge) : this.geometry.nlambdasCCW(flatEdge);

                result.color = new Trianglef(
                    new Vec3f(this.color.v0),
                    new Vec3f(
                        this.color.v0.x * w.x + this.color.v1.x * w.y + this.color.v2.x * w.z,
                        this.color.v0.y * w.x + this.color.v1.y * w.y + this.color.v2.y * w.z,
                        this.color.v0.z * w.x + this.color.v1.z * w.y + this.color.v2.z * w.z
                    ),
                    new Vec3f(this.color.v1)
                );

                if (this.textureCoords.v0.x != 0.0f) {
                    string fs = "";
                }

                result.textureCoords = new Trianglef(
                    new Vec3f(this.textureCoords.v0),
                    new Vec3f(
                        this.textureCoords.v0.x * w.x + this.textureCoords.v1.x * w.y + this.textureCoords.v2.x * w.z,
                        this.textureCoords.v0.y * w.x + this.textureCoords.v1.y * w.y + this.textureCoords.v2.y * w.z,
                        this.textureCoords.v0.z * w.x + this.textureCoords.v1.z * w.y + this.textureCoords.v2.z * w.z
                    ),
                    new Vec3f(this.textureCoords.v1)
                );
            }
            else {

                Trianglef temp = this.geometry.reSort();
                Vec2f flatEdge = temp.flatCoeff();
                result.geometry = temp.getBottomFlat(flatEdge);

                bool clockWise = temp.clockWise();
                Vec3f w = temp.clockWise() ? temp.nlambdas(flatEdge) : temp.nlambdasCCW(flatEdge);

                result.color = new Trianglef(
                    new Vec3f(this.color[temp.order[0]]),
                    new Vec3f(
                        this.color[temp.order[0]].x * w.x + this.color[temp.order[1]].x * w.y + this.color[temp.order[2]].x * w.z,
                        this.color[temp.order[0]].y * w.x + this.color[temp.order[1]].y * w.y + this.color[temp.order[2]].y * w.z,
                        this.color[temp.order[0]].z * w.x + this.color[temp.order[1]].z * w.y + this.color[temp.order[2]].z * w.z
                    ),
                    new Vec3f(this.color[temp.order[1]])
                );
                result.textureCoords = new Trianglef(
                    new Vec3f(this.textureCoords[temp.order[0]]),
                    new Vec3f(
                        this.textureCoords[temp.order[0]].x * w.x + this.textureCoords[temp.order[1]].x * w.y + this.textureCoords[temp.order[2]].x * w.z,
                        this.textureCoords[temp.order[0]].y * w.x + this.textureCoords[temp.order[1]].y * w.y + this.textureCoords[temp.order[2]].y * w.z,
                        this.textureCoords[temp.order[0]].z * w.x + this.textureCoords[temp.order[1]].z * w.y + this.textureCoords[temp.order[2]].z * w.z
                    ),
                    new Vec3f(this.textureCoords[temp.order[1]])
                );
            }

            return result;
        }

        public RPolygon getTopFlat() {

            RPolygon result = new RPolygon();

            if (this.geometry.sorted) {

                Vec2f flatEdge = this.geometry.flatCoeff();
                result.geometry = this.geometry.getTopFlat(flatEdge);

                bool clockWise = this.geometry.clockWise();
                Vec3f w = this.geometry.clockWise() ? this.geometry.nlambdas(flatEdge) : this.geometry.nlambdasCCW(flatEdge);

                result.color = new Trianglef(
                    new Vec3f(this.color.v1),
                    new Vec3f(
                        this.color.v0.x * w.x + this.color.v1.x * w.y + this.color.v2.x * w.z,
                        this.color.v0.y * w.x + this.color.v1.y * w.y + this.color.v2.y * w.z,
                        this.color.v0.z * w.x + this.color.v1.z * w.y + this.color.v2.z * w.z
                    ),
                    new Vec3f(this.color.v2)
                );
                result.textureCoords = new Trianglef(
                    new Vec3f(this.textureCoords.v1),
                    new Vec3f(
                        this.textureCoords.v0.x * w.x + this.textureCoords.v1.x * w.y + this.textureCoords.v2.x * w.z,
                        this.textureCoords.v0.y * w.x + this.textureCoords.v1.y * w.y + this.textureCoords.v2.y * w.z,
                        this.textureCoords.v0.z * w.x + this.textureCoords.v1.z * w.y + this.textureCoords.v2.z * w.z
                    ),
                    new Vec3f(this.textureCoords.v2)
                );
            }
            else {

                Trianglef temp = this.geometry.reSort();
                Vec2f flatEdge = temp.flatCoeff();
                result.geometry = temp.getBottomFlat(flatEdge);

                bool clockWise = temp.clockWise();
                Vec3f w = temp.clockWise() ? temp.nlambdas(flatEdge) : temp.nlambdasCCW(flatEdge);

                result.color = new Trianglef(
                    new Vec3f(this.color[temp.order[1]]),
                    new Vec3f(
                        this.color[temp.order[0]].x * w.x + this.color[temp.order[1]].x * w.y + this.color[temp.order[2]].x * w.z,
                        this.color[temp.order[0]].y * w.x + this.color[temp.order[1]].y * w.y + this.color[temp.order[2]].y * w.z,
                        this.color[temp.order[0]].z * w.x + this.color[temp.order[1]].z * w.y + this.color[temp.order[2]].z * w.z
                    ),
                    new Vec3f(this.color[temp.order[2]])
                );
                result.textureCoords = new Trianglef(
                    new Vec3f(this.textureCoords[temp.order[1]]),
                    new Vec3f(
                        this.textureCoords[temp.order[0]].x * w.x + this.textureCoords[temp.order[1]].x * w.y + this.textureCoords[temp.order[2]].x * w.z,
                        this.textureCoords[temp.order[0]].y * w.x + this.textureCoords[temp.order[1]].y * w.y + this.textureCoords[temp.order[2]].y * w.z,
                        this.textureCoords[temp.order[0]].z * w.x + this.textureCoords[temp.order[1]].z * w.y + this.textureCoords[temp.order[2]].z * w.z
                    ),
                    new Vec3f(this.textureCoords[temp.order[2]])
                );
            }

            return result;
        }
    }
}
