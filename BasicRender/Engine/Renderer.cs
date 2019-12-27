using MathematicalEntities;
using ObjReader.Data.Elements;
using ObjReader.Loaders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BasicRender.Engine {

    public class Renderer {

        public byte[] buf;
        public float[] zbuf;
        public int pixelWidth;
        public float pixelWidthf;
        public int stride;
        public int pixelHeight;
        public float pixelHeightf;
        public Vec2f pixelLeftTop;
        public Vec2f pixelRightBottom;

        public byte[] texture;
        public int textureWidth;
        public float textureWidthf;
        public int textureStride;
        public int textureHeight;
        public float textureHeightf;

        public LoadResult loadResult;
        public GameTimer timer;

        public Mat4f model;
        public Mat4f world;

        public Renderer(GameTimer timer, byte[] buf, float[] zbuf, int pixelWidth) {

            this.buf = buf;
            this.zbuf = zbuf;
            this.pixelWidth = pixelWidth;
            this.pixelWidthf = this.pixelWidth;

            this.stride = (pixelWidth * 32) / 8;
            this.pixelHeight = buf.Length / stride;
            this.pixelHeightf = this.pixelHeight;

            this.pixelLeftTop = new Vec2f(0.0f, 0.0f);
            this.pixelRightBottom = new Vec2f(this.pixelWidthf, this.pixelHeightf);

            this.timer = timer;
        }

        public void setObject(LoadResult obj) {
            this.loadResult = obj;
        }

        private string getMdlFileName(string fileName) {

            string result = "";
            string bmpFileName = Path.GetFileNameWithoutExtension(fileName);
            string[] ss = bmpFileName.Split('.');

            for (int i = 0; i < ss.Length; i++) {
                if (ss[i] == "mdl_a2") {
                    result += ss[i];
                    break;
                }
                result += ss[i] + ".";
            }

            return result;
        }

        public void setObject(string fileName) {

            this.loadResult = ObjLoaderFactory.Create().Load(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None));
            string bmpFileName = getMdlFileName(Path.GetFileNameWithoutExtension(fileName)) + ".bmp";

            if (File.Exists(Path.GetFullPath(bmpFileName))) {
                Bitmap bmp = new Bitmap(bmpFileName);
                this.textureWidth = bmp.Width;
                this.textureWidthf = this.textureWidth;

                this.textureStride = (textureWidth * 32) / 8;
                this.textureHeight = bmp.Height;
                this.textureHeightf = this.textureHeight;

                this.texture = new byte[this.textureWidth * this.textureHeight * 4];

                for (int x = 0; x < bmp.Width; x++) {
                    for (int y = 0; y < bmp.Height; y++) {

                        Color pxl = bmp.GetPixel(x, y);

                        byte red = pxl.R;
                        byte green = pxl.G;
                        byte blue = pxl.B;
                        byte alpha = pxl.A;

                        int pixelOffset = (x + y * textureWidth) * 32 / 8;
                        this.texture[pixelOffset] = blue;
                        this.texture[pixelOffset + 1] = green;
                        this.texture[pixelOffset + 2] = red;
                        this.texture[pixelOffset + 3] = alpha;
                    }
                }
            }
        }

        public void setModel(Mat4f model) {
            this.model = model;
        }

        public void setWorld(Mat4f world) {
            this.world = world;
        }

        public int countFrames() {
            return loadResult.Groups.Count;
        }

        public void printPixel(int x, int y, Vec3f color) {

            byte red = (byte)(color.x * 255.0f);
            byte green = (byte)(color.y * 255.0f);
            byte blue = (byte)(color.z * 255.0f);
            byte alpha = 255;

            int pixelOffset = (x + y * pixelWidth) * 32 / 8;
            buf[pixelOffset] = blue;
            buf[pixelOffset + 1] = green;
            buf[pixelOffset + 2] = red;
            buf[pixelOffset + 3] = alpha;
        }

        public void printPixelZ(int x, int y, float z, Vec3f color) {

            byte red = (byte)(color.x * 255.0f);
            byte green = (byte)(color.y * 255.0f);
            byte blue = (byte)(color.z * 255.0f);
            byte alpha = 255;

            int offset = (x + y * pixelWidth);
            float oneOverZ = 1.0f / z;

            if (zbuf[offset] > oneOverZ) {

                zbuf[offset] = oneOverZ;

                int pixelOffset = (x + y * pixelWidth) * 32 / 8;
                buf[pixelOffset] = blue;
                buf[pixelOffset + 1] = green;
                buf[pixelOffset + 2] = red;
                buf[pixelOffset + 3] = alpha;
            }
        }

        public void fillScreen(Vec3f color) {

            for (int y = 0; y < pixelHeight; y++)
                for (int x = 0; x < pixelWidth; x++)
                    printPixel(x, y, color);
        }

        public void fillZBuff(float z) {

            for (int y = 0; y < pixelHeight; y++) {
                for (int x = 0; x < pixelWidth; x++) {
                    int offset = (x + y * pixelWidth);
                    zbuf[offset] = z;
                }
            }
        }

        public void lmoveScreen(Vec3f fillColor, int moveAmt) {

            for (int y = 0; y < pixelHeight; y++) {
                for (int x = 0; x < pixelWidth; x++) {

                    int nextPixel = x + moveAmt;
                    if (nextPixel < pixelWidth) {
                        int pixelOffset = (x + y * pixelWidth) * 32 / 8;
                        int pixelOffsetNew = (nextPixel + y * pixelWidth) * 32 / 8;
                        buf[pixelOffset] = buf[pixelOffsetNew];
                        buf[pixelOffset + 1] = buf[pixelOffsetNew + 1];
                        buf[pixelOffset + 2] = buf[pixelOffsetNew + 2];
                        buf[pixelOffset + 3] = buf[pixelOffsetNew + 3];
                    }
                    else {
                        printPixel(x, y, fillColor);
                    }
                }
            }
        }

        public void printLine(Linef lineCoords, Vec3f color) {

            int stride = (pixelWidth * 32) / 8;
            int pixelHeight = buf.Length / stride;

            int x0 = (int)lineCoords.x0;
            int y0 = (int)lineCoords.y0;
            int x1 = (int)lineCoords.x1;
            int y1 = (int)lineCoords.y1;

            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;

            int dy = Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;

            int err = (dx > dy ? dx : -dy) / 2;
            int e2;

            for (; ; ) {

                printPixel(x0, y0, color);

                if (x0 == x1 && y0 == y1)
                    break;

                e2 = err;

                if (e2 > -dx) {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dy) {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        public void printTriangleWireframe(Trianglef triangle, Vec3f color) {

            printLine(new Linef(triangle.v0, triangle.v1), color);
            printLine(new Linef(triangle.v1, triangle.v2), color);
            printLine(new Linef(triangle.v2, triangle.v0), color);
        }

        public void printTriangleFlat(Trianglef triangle, Vec3f color) {

            Trianglef t1 = triangle.reSort();
            Vec2i A = Vec2i.fromCeil(t1.v0);
            Vec2i B = Vec2i.fromCeil(t1.v1);
            Vec2i C = Vec2i.fromCeil(t1.v2);

            int x1;
            int x2;

            for (int sy = A.y; sy < C.y; sy++) {
                x1 = A.x + (sy - A.y) * (C.x - A.x) / (C.y - A.y);
                if (sy < B.y)
                    x2 = A.x + (sy - A.y) * (B.x - A.x) / (B.y - A.y);
                else {
                    if (C.y == B.y)
                        x2 = B.x;
                    else
                        x2 = B.x + (sy - B.y) * (C.x - B.x) / (C.y - B.y);
                }
                if (x1 > x2) { int tmp = x1; x1 = x2; x2 = tmp; }

                for (int i = x1; i < x2; i++) {
                    printPixel(i, sy, color);
                }
            }
        }

        public void printTriangleFlatZ(Trianglef triangle, Vec3f color) {

            Trianglef t1 = triangle.reSort();

            Vec2i A = Vec2i.fromCeil(t1.v0);
            float Az = t1.v0.z;

            Vec2i B = Vec2i.fromCeil(t1.v1);
            float Bz = t1.v1.z;

            Vec2i C = Vec2i.fromCeil(t1.v2);
            float Cz = t1.v2.z;

            int x1;
            int x2;

            for (int sy = A.y; sy < C.y; sy++) {


                x1 = A.x + (sy - A.y) * (C.x - A.x) / (C.y - A.y);

                if (sy < B.y)
                    x2 = A.x + (sy - A.y) * (B.x - A.x) / (B.y - A.y);
                else {
                    if (C.y == B.y)
                        x2 = B.x;
                    else
                        x2 = B.x + (sy - B.y) * (C.x - B.x) / (C.y - B.y);
                }

                if (x1 > x2) {
                    int tmp = x1;
                    x1 = x2;
                    x2 = tmp;
                }


                for (int i = x1; i < x2; i++) {

                    Vec3f w = t1.nlambdas(new Vec2f(i, sy));
                    printPixelZ(i, sy, Az * w.x + Bz * w.y + Cz * w.z, color);
                }
            }
        }

        public void printPolyFlatZ2(RPolygon poly) {

            Trianglef t1 = poly.geometry.reSort();

            Vec2i A = Vec2i.fromCeil(t1.v0);
            float Az = t1.v0.z;

            Vec2i B = Vec2i.fromCeil(t1.v1);
            float Bz = t1.v1.z;

            Vec2i C = Vec2i.fromCeil(t1.v2);
            float Cz = t1.v2.z;

            int x1;
            int x2;

            bool clockWise = true;

            for (int sy = A.y; sy < C.y; sy++) {


                x1 = A.x + (sy - A.y) * (C.x - A.x) / (C.y - A.y);

                if (sy < B.y)
                    x2 = A.x + (sy - A.y) * (B.x - A.x) / (B.y - A.y);
                else {
                    if (C.y == B.y)
                        x2 = B.x;
                    else
                        x2 = B.x + (sy - B.y) * (C.x - B.x) / (C.y - B.y);
                }

                if (x1 > x2) {
                    int tmp = x1;
                    x1 = x2;
                    x2 = tmp;
                    clockWise = false;
                }


                for (int i = x1; i < x2; i++) {

                    if (clockWise) {
                        Vec3f w = t1.nlambdas(new Vec2f(i, sy));
                        printPixelZ(
                            i,
                            sy,
                            Az * w.x + Bz * w.y + Cz * w.z,
                            poly.color[t1.order[0]] * w.x + poly.color[t1.order[1]] * w.y + poly.color[t1.order[2]] * w.z
                        );
                    }
                    else {
                        Vec3f w = t1.nlambdas(new Vec2f(i, sy));
                        printPixelZ(
                            i,
                            sy,
                            Az * w.x + Bz * w.y + Cz * w.z,
                            poly.color[t1.order[0]] * w.x + poly.color[t1.order[1]] * w.y + poly.color[t1.order[2]] * w.z
                        );
                    }
                }
            }
        }

        public void printPolyFlatZ(RPolygon pIn) {

            if (pIn.geometry.isEpsilonGeometry())
                return;

            RPolygon polygon = pIn.reSort();

            if (polygon.geometry.isOffScreen(this.pixelLeftTop, this.pixelRightBottom))
                return;

            if (polygon.geometry.isFlatTop()) {
                printPolyFlatTopZ(polygon);
            }
            else {
                if (polygon.geometry.isFlatBottom()) {
                    printPolyFlatBottomZ(polygon);
                }
                else {

                    RPolygon p1 = polygon.getBottomFlat();
                    printPolyFlatBottomZ(p1);

                    RPolygon p2 = polygon.getTopFlat();
                    printPolyFlatTopZ(p2);
                }
            }
        }

        public void printPolyFlatZ3(RPolygon poly) {

            if (poly.geometry.isEpsilonGeometry())
                return;

            Trianglef triangle = poly.geometry.reSort();

            if (triangle.isOffScreen(this.pixelLeftTop, this.pixelRightBottom))
                return;

            Trianglef original = poly.geometry;
            if (triangle.isFlatTop()) {
                poly.geometry = triangle;
                printPolyFlatTopZ(poly);
            }
            else {
                if (triangle.isFlatBottom()) {
                    poly.geometry = triangle;
                    printPolyFlatBottomZ(poly);
                }
                else {

                    Vec2f flatEdge = triangle.flatCoeff();

                    poly.geometry = triangle.getBottomFlat(flatEdge);
                    printPolyFlatBottomZ(poly);

                    poly.geometry = triangle.getTopFlat(flatEdge);
                    printPolyFlatTopZ(poly);
                }
            }
            poly.geometry = original;
        }

        public Vec3f sampleTexture(Vec2f UV) {

            if (UV.u * this.textureWidthf > this.textureWidthf)
                UV.u = 1.0f;

            if (UV.u < 0.0f)
                UV.u = 0.0f;

            if (UV.v * this.textureHeightf > this.textureHeightf)
                UV.v = 1.0f;

            if (UV.v < 0.0f)
                UV.v = 0.0f;

            if((UV.u) * textureWidthf < 22.0f) {
                string fwsv = "";
            }

            int pixelOffset = ((int)((UV.u) * textureWidthf) + (int)((1.0f - UV.v) * textureHeightf) * textureHeight) * 32 / 8;

            byte red = texture[pixelOffset + 2];
            byte green = texture[pixelOffset + 1];
            byte blue = texture[pixelOffset];

            return new Vec3f((float)red / 255.0f, (float)green / 255.0f, (float)blue / 255.0f);
        }

        public void printPolyFlatBottomZ(RPolygon poly) {

            Vec3f v0 = new Vec3f(poly.geometry.v0);
            Vec3f v1 = new Vec3f(poly.geometry.v1);
            Vec3f v2 = new Vec3f(poly.geometry.v2);

            bool clockWise = v2.x < v1.x ? true : false;

            if (clockWise) {
                Vec3f temp = v1;
                v1 = v2;
                v2 = temp;
            }

            float height = v2.y - v0.y;

            float dx_left = (v1.x - v0.x) / height;
            float dx_right = (v2.x - v0.x) / height;

            float xStart = v0.x;
            float xEnd = v0.x;

            int iy1;
            int iy3;
            int loop_y;

            if (v0.y < 0.0f) {

                xStart = xStart + dx_left * (-v0.y);
                xEnd = xEnd + dx_right * (-v0.y);

                v0.y = 0.0f;
                iy1 = 0;
            }
            else {

                iy1 = (int)Math.Ceiling(v0.y);

                xStart = xStart + dx_left * ((float)iy1 - v0.y);
                xEnd = xEnd + dx_right * ((float)iy1 - v0.y);
            }

            if (v2.y > pixelHeight) {

                v2.y = pixelHeight;
                iy3 = (int)v2.y - 1;
            }
            else {
                iy3 = (int)Math.Ceiling(v2.y) - 1;
            }

            float yTemp = v0.y;
            if (v0.x >= 0 && v0.x < pixelWidth && v1.x >= 0 && v1.x < pixelWidth && v2.x >= 0 && v2.x < pixelWidth) {

                for (loop_y = iy1; loop_y <= iy3; loop_y++) {

                    int ixStart = (int)xStart;
                    int ixEnd = (int)xEnd;
                    float xTemp = xStart;

                    for (int x = ixStart; x < ixEnd; x++) {

                        Vec2f p = new Vec2f(xTemp, loop_y);
                        Vec3f w = clockWise ? poly.geometry.nlambdas(p) : poly.geometry.nlambdasCCW(p);

                        Vec3f color = new Vec3f(
                            poly.color.v0.x * w.x + poly.color.v1.x * w.y + poly.color.v2.x * w.z,
                            poly.color.v0.y * w.x + poly.color.v1.y * w.y + poly.color.v2.y * w.z,
                            poly.color.v0.z * w.x + poly.color.v1.z * w.y + poly.color.v2.z * w.z
                        );

                        Vec2f UV = new Vec2f(
                            poly.textureCoords.v0.x * w.x + poly.textureCoords.v1.x * w.y + poly.textureCoords.v2.x * w.z,
                            poly.textureCoords.v0.y * w.x + poly.textureCoords.v1.y * w.y + poly.textureCoords.v2.y * w.z
                        );

                        bool alphaColor = false;
                        if (UV.x != 0.0f && UV.y != 0.0f) {
                            Vec3f texColor = sampleTexture(UV);
                            if (texColor.x != 0.0f && texColor.y != 0.0f && texColor.z != 0.0f) {
                                color.x = (color.x + texColor.x) / 2.0f;
                                color.y = (color.y + texColor.y) / 2.0f;
                                color.z = (color.z + texColor.z) / 2.0f;
                            }
                            else {
                                alphaColor = true;
                            }
                        }

                        if (!alphaColor) {
                            printPixelZ(
                                x,
                                loop_y,
                                poly.geometry.v0.z * w.x + poly.geometry.v1.z * w.y + poly.geometry.v2.z * w.z,
                                color
                            );
                        }

                        xTemp += 1.0f;
                    }

                    xStart += dx_left;
                    xEnd += dx_right;
                    yTemp += 1.0f;
                }
            }
            else {

                for (loop_y = iy1; loop_y <= iy3; loop_y++) {

                    float left = xStart;
                    float right = xEnd;

                    xStart += dx_left;
                    xEnd += dx_right;

                    if (left < 0) {
                        left = 0;
                        if (right < 0)
                            continue;
                    }

                    if (right > pixelWidth) {
                        right = pixelWidth;
                        if (left > pixelWidth)
                            continue;
                    }

                    int ixStart = (int)left;
                    int ixEnd = (int)right;
                    float xTemp = left;

                    for (int x = ixStart; x < ixEnd; x++) {

                        Vec2f p = new Vec2f(xTemp, loop_y);
                        Vec3f w = clockWise ? poly.geometry.nlambdas(p) : poly.geometry.nlambdasCCW(p);

                        Vec3f color = new Vec3f(
                            poly.color.v0.x * w.x + poly.color.v1.x * w.y + poly.color.v2.x * w.z,
                            poly.color.v0.y * w.x + poly.color.v1.y * w.y + poly.color.v2.y * w.z,
                            poly.color.v0.z * w.x + poly.color.v1.z * w.y + poly.color.v2.z * w.z
                        );

                        printPixelZ(
                            x,
                            loop_y,
                            poly.geometry.v0.z * w.x + poly.geometry.v1.z * w.y + poly.geometry.v2.z * w.z,
                            color
                        );

                        xTemp += 1.0f;
                    }

                    yTemp += 1.0f;
                }
            }
        }

        public void printPolyFlatTopZ(RPolygon poly) {

            Vec3f v0 = new Vec3f(poly.geometry.v0);
            Vec3f v1 = new Vec3f(poly.geometry.v1);
            Vec3f v2 = new Vec3f(poly.geometry.v2);

            bool clockWise = v1.x > v0.x ? true : false;

            if (!clockWise) {
                Vec3f temp = v1;
                v1 = v0;
                v0 = temp;
            }

            float height = v2.y - v0.y;

            float dx_left = (v2.x - v0.x) / height;
            float dx_right = (v2.x - v1.x) / height;

            float xStart = v0.x;
            float xEnd = v1.x;

            int iy1;
            int iy3;
            int loop_y;

            if (v0.y < 0.0f) {

                xStart = xStart + dx_left * (-v0.y);
                xEnd = xEnd + dx_right * (-v0.y);

                v0.y = 0.0f;
                iy1 = 0;
            }
            else {

                iy1 = (int)Math.Ceiling(v0.y);

                xStart = xStart + dx_left * ((float)iy1 - v0.y);
                xEnd = xEnd + dx_right * ((float)iy1 - v0.y);
            }

            if (v2.y > pixelHeight) {

                v2.y = pixelHeight;
                iy3 = (int)v2.y - 1;
            }
            else {
                iy3 = (int)Math.Ceiling(v2.y) - 1;
            }

            float yTemp = v0.y;
            if (v0.x >= 0 && v0.x < pixelWidth && v1.x >= 0 && v1.x < pixelWidth && v2.x >= 0 && v2.x < pixelWidth) {
                
                for (loop_y = iy1; loop_y <= iy3; loop_y++) {

                    int ixStart = (int)xStart;
                    int ixEnd = (int)xEnd;
                    float xTemp = xStart;

                    for (int x = ixStart; x < ixEnd; x++) {

                        Vec2f p = new Vec2f(xTemp, loop_y);
                        Vec3f w = clockWise ? poly.geometry.nlambdas(p) : poly.geometry.nlambdasCCW(p);

                        Vec3f color = new Vec3f(
                            poly.color.v0.x * w.x + poly.color.v1.x * w.y + poly.color.v2.x * w.z,
                            poly.color.v0.y * w.x + poly.color.v1.y * w.y + poly.color.v2.y * w.z,
                            poly.color.v0.z * w.x + poly.color.v1.z * w.y + poly.color.v2.z * w.z
                        );

                        Vec2f UV = new Vec2f(
                            poly.textureCoords.v0.x * w.x + poly.textureCoords.v1.x * w.y + poly.textureCoords.v2.x * w.z,
                            poly.textureCoords.v0.y * w.x + poly.textureCoords.v1.y * w.y + poly.textureCoords.v2.y * w.z
                        );

                        bool alphaColor = false;
                        if (UV.x != 0.0f && UV.y != 0.0f) {
                            Vec3f texColor = sampleTexture(UV);
                            if (texColor.x != 0.0f && texColor.y != 0.0f && texColor.z != 0.0f) {
                                color.x = (color.x + texColor.x) / 2.0f;
                                color.y = (color.y + texColor.y) / 2.0f;
                                color.z = (color.z + texColor.z) / 2.0f;
                            }
                            else {
                                alphaColor = true;
                            }
                        }

                        if (!alphaColor) {
                            printPixelZ(
                                x,
                                loop_y,
                                poly.geometry.v0.z * w.x + poly.geometry.v1.z * w.y + poly.geometry.v2.z * w.z,
                                color
                            );
                        }

                        xTemp += 1.0f;
                    }

                    xStart += dx_left;
                    xEnd += dx_right;
                    yTemp += 1.0f;
                }
            }
            else {

                for (loop_y = iy1; loop_y <= iy3; loop_y++) {

                    float left = xStart;
                    float right = xEnd;

                    xStart += dx_left;
                    xEnd += dx_right;

                    if (left < 0) {
                        left = 0;
                        if (right < 0)
                            continue;
                    }

                    if (right > pixelWidth) {
                        right = pixelWidth;
                        if (left > pixelWidth)
                            continue;
                    }

                    int ixStart = (int)left;
                    int ixEnd = (int)right;
                    float xTemp = left;

                    for (int x = ixStart; x < ixEnd; x++) {

                        Vec2f p = new Vec2f(xTemp, loop_y);
                        Vec3f w = clockWise ? poly.geometry.nlambdas(p) : poly.geometry.nlambdasCCW(p);

                        Vec3f color = new Vec3f(
                            poly.color.v0.x * w.x + poly.color.v1.x * w.y + poly.color.v2.x * w.z,
                            poly.color.v0.y * w.x + poly.color.v1.y * w.y + poly.color.v2.y * w.z,
                            poly.color.v0.z * w.x + poly.color.v1.z * w.y + poly.color.v2.z * w.z
                        );

                        printPixelZ(
                            x,
                            loop_y,
                            poly.geometry.v0.z * w.x + poly.geometry.v1.z * w.y + poly.geometry.v2.z * w.z,
                            color
                        );

                        xTemp += 1.0f;
                    }

                    yTemp += 1.0f;
                }
            }
        }



        public void renderPolySolidColorZ(int frame) {

            Mat4f modelWorld = model * world;
            Mat4f proj = Mat4f.PerspectiveRH(GeneralVariables.M_PI / 3.0f, 320.0f / 200.0f, 0.1f, 1000.0f);

            foreach (Face face in loadResult.Groups[frame].Faces) {

                if (face.Count == 3) {

                    Vec4f point1Original = new Vec4f(
                        loadResult.Vertices[face[0].VertexIndex - 1].X,
                        loadResult.Vertices[face[0].VertexIndex - 1].Y,
                        loadResult.Vertices[face[0].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point1 = modelWorld * point1Original;
                    point1 = proj * point1;
                    point1.x = (point1.x + 1.0f) / 2.0f * 320.0f;
                    point1.y = (point1.y + 1.0f) / 2.0f * 200.0f;
                    //point1.z = point1Original.z;
                    Vec3f color1 = new Vec3f(
                        loadResult.Vertices[face[0].VertexIndex - 1].R,
                        loadResult.Vertices[face[0].VertexIndex - 1].G,
                        loadResult.Vertices[face[0].VertexIndex - 1].B
                    );
                    Vec3f texColor1 = new Vec3f(
                        face[0].TextureIndex != 0 ? loadResult.Textures[face[0].TextureIndex - 1].X : 0.0f,
                        face[0].TextureIndex != 0 ? loadResult.Textures[face[0].TextureIndex - 1].Y : 0.0f,
                        0.0f
                    );

                    Vec4f point2Original = new Vec4f(
                        loadResult.Vertices[face[1].VertexIndex - 1].X,
                        loadResult.Vertices[face[1].VertexIndex - 1].Y,
                        loadResult.Vertices[face[1].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point2 = modelWorld * point2Original;
                    point2 = proj * point2;
                    point2.x = (point2.x + 1.0f) / 2.0f * 320.0f;
                    point2.y = (point2.y + 1.0f) / 2.0f * 200.0f;
                    //point2.z = point2Original.z;
                    Vec3f color2 = new Vec3f(
                        loadResult.Vertices[face[1].VertexIndex - 1].R,
                        loadResult.Vertices[face[1].VertexIndex - 1].G,
                        loadResult.Vertices[face[1].VertexIndex - 1].B
                    );
                    Vec3f texColor2 = new Vec3f(
                        face[1].TextureIndex != 0 ? loadResult.Textures[face[1].TextureIndex - 1].X : 0.0f,
                        face[1].TextureIndex != 0 ? loadResult.Textures[face[1].TextureIndex - 1].Y : 0.0f,
                        0.0f
                    );

                    Vec4f point3Original = new Vec4f(
                        loadResult.Vertices[face[2].VertexIndex - 1].X,
                        loadResult.Vertices[face[2].VertexIndex - 1].Y,
                        loadResult.Vertices[face[2].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point3 = modelWorld * point3Original;
                    point3 = proj * point3;
                    point3.x = (point3.x + 1.0f) / 2.0f * 320.0f;
                    point3.y = (point3.y + 1.0f) / 2.0f * 200.0f;
                    //point3.z = point3Original.z;
                    Vec3f color3 = new Vec3f(
                        loadResult.Vertices[face[2].VertexIndex - 1].R,
                        loadResult.Vertices[face[2].VertexIndex - 1].G,
                        loadResult.Vertices[face[2].VertexIndex - 1].B
                    );
                    Vec3f texColor3 = new Vec3f(
                        face[2].TextureIndex != 0 ? loadResult.Textures[face[2].TextureIndex - 1].X : 0.0f,
                        face[2].TextureIndex != 0 ? loadResult.Textures[face[2].TextureIndex - 1].Y : 0.0f,
                        0.0f
                    );

                    //Vec4f point1Original = new Vec4f(
                    //    0.1f,
                    //    0.1f,
                    //    0.0f,
                    //    1.0f
                    //);
                    //Vec4f point1 = modelWorld * point1Original;
                    //point1 = proj * point1;
                    //point1.x = (point1.x + 1.0f) / 2.0f * 320.0f;
                    //point1.y = (point1.y + 1.0f) / 2.0f * 200.0f;
                    ////point1.z = point1Original.z;
                    //Vec3f color1 = new Vec3f(
                    //    1.0f,
                    //    0.0f,
                    //    0.0f
                    //);

                    //Vec4f point2Original = new Vec4f(
                    //    0.9f,
                    //    0.4f,
                    //    0.0f,
                    //    1.0f
                    //);
                    //Vec4f point2 = modelWorld * point2Original;
                    //point2 = proj * point2;
                    //point2.x = (point2.x + 1.0f) / 2.0f * 320.0f;
                    //point2.y = (point2.y + 1.0f) / 2.0f * 200.0f;
                    ////point2.z = point2Original.z;
                    //Vec3f color2 = new Vec3f(
                    //    0.0f,
                    //    1.0f,
                    //    0.0f
                    //);

                    //Vec4f point3Original = new Vec4f(
                    //    0.4f,
                    //    0.9f,
                    //    0.0f,
                    //    1.0f
                    //);
                    //Vec4f point3 = modelWorld * point3Original;
                    //point3 = proj * point3;
                    //point3.x = (point3.x + 1.0f) / 2.0f * 320.0f;
                    //point3.y = (point3.y + 1.0f) / 2.0f * 200.0f;
                    ////point3.z = point3Original.z;
                    //Vec3f color3 = new Vec3f(
                    //    0.0f,
                    //    0.0f,
                    //    1.0f
                    //);

                    Trianglef triangle3Original = new Trianglef(point1Original, point2Original, point3Original);
                    Trianglef triangle3 = new Trianglef(point1, point2, point3);

                    Vec3f worldV = new Vec3f(modelWorld * triangle3Original.v0);
                    Trianglef triangle = new Trianglef(modelWorld * point1Original, modelWorld * point2Original, modelWorld * point3Original);
                    Vec3f N = triangle.tangent();
                    float cosTheta = worldV.dot(N);
                    bool visible = cosTheta > 0.0f;

                    if (visible) {
                        //if (color1.x != 0.0f) {
                            printPolyFlatZ(
                                new RPolygon(
                                    triangle3,
                                    new Trianglef(color1, color2, color3),
                                    new Trianglef(texColor1, texColor2, texColor3)
                                )
                            );
                        //}
                        //else {
                        //    printPolyFlatZ(
                        //        new RPolygon(
                        //            triangle3,
                        //            new Trianglef(new Vec3f(0.671f, 0.345f, 0.745f), new Vec3f(0.671f, 0.345f, 0.745f), new Vec3f(0.671f, 0.345f, 0.745f)),
                        //            new Trianglef()
                        //        )
                        //    );
                        //}
                    }
                }
                else {

                    Vec4f point1Original = new Vec4f(
                        loadResult.Vertices[face[0].VertexIndex - 1].X,
                        loadResult.Vertices[face[0].VertexIndex - 1].Y,
                        loadResult.Vertices[face[0].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point1 = modelWorld * point1Original;
                    point1 = proj * point1;
                    point1.x = (point1.x + 1.0f) / 2.0f * 320.0f;
                    point1.y = (point1.y + 1.0f) / 2.0f * 200.0f;
                    Vec3f color1 = new Vec3f(
                        loadResult.Vertices[face[0].VertexIndex - 1].R,
                        loadResult.Vertices[face[0].VertexIndex - 1].G,
                        loadResult.Vertices[face[0].VertexIndex - 1].B
                    );

                    Vec4f point2Original = new Vec4f(
                        loadResult.Vertices[face[1].VertexIndex - 1].X,
                        loadResult.Vertices[face[1].VertexIndex - 1].Y,
                        loadResult.Vertices[face[1].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point2 = modelWorld * point2Original;
                    point2 = proj * point2;
                    point2.x = (point2.x + 1.0f) / 2.0f * 320.0f;
                    point2.y = (point2.y + 1.0f) / 2.0f * 200.0f;
                    Vec3f color2 = new Vec3f(
                        loadResult.Vertices[face[1].VertexIndex - 1].R,
                        loadResult.Vertices[face[1].VertexIndex - 1].G,
                        loadResult.Vertices[face[1].VertexIndex - 1].B
                    );

                    Vec4f point3Original = new Vec4f(
                        loadResult.Vertices[face[2].VertexIndex - 1].X,
                        loadResult.Vertices[face[2].VertexIndex - 1].Y,
                        loadResult.Vertices[face[2].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point3 = modelWorld * point3Original;
                    point3 = proj * point3;
                    point3.x = (point3.x + 1.0f) / 2.0f * 320.0f;
                    point3.y = (point3.y + 1.0f) / 2.0f * 200.0f;
                    Vec3f color3 = new Vec3f(
                        loadResult.Vertices[face[2].VertexIndex - 1].R,
                        loadResult.Vertices[face[2].VertexIndex - 1].G,
                        loadResult.Vertices[face[2].VertexIndex - 1].B
                    );

                    Vec4f point4Original = new Vec4f(
                        loadResult.Vertices[face[3].VertexIndex - 1].X,
                        loadResult.Vertices[face[3].VertexIndex - 1].Y,
                        loadResult.Vertices[face[3].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point4 = modelWorld * point4Original;
                    point4 = proj * point4;
                    point4.x = (point4.x + 1.0f) / 2.0f * 320.0f;
                    point4.y = (point4.y + 1.0f) / 2.0f * 200.0f;
                    Vec3f color4 = new Vec3f(
                        loadResult.Vertices[face[3].VertexIndex - 1].R,
                        loadResult.Vertices[face[3].VertexIndex - 1].G,
                        loadResult.Vertices[face[3].VertexIndex - 1].B
                    );

                    Trianglef triangle1Original = new Trianglef(point1Original, point2Original, point3Original);
                    Trianglef triangle2Original = new Trianglef(point1Original, point3Original, point4Original);
                    Trianglef triangle1 = new Trianglef(point1, point2, point3);
                    Trianglef triangle2 = new Trianglef(point1, point3, point4);

                    Vec3f worldV = new Vec3f(modelWorld * triangle1Original.v0);
                    Trianglef triangle = new Trianglef(modelWorld * point1Original, modelWorld * point2Original, modelWorld * point3Original);
                    Vec3f N = triangle.tangent();
                    float cosTheta = worldV.dot(N);
                    bool visible = cosTheta < 0.0f;

                    //if (visible) {

                        if (color1.x != 0.0f) {
                            printPolyFlatZ(
                                new RPolygon(
                                    triangle1,
                                    new Trianglef(color1, color2, color3),
                                    new Trianglef()
                                )
                            );
                            printPolyFlatZ(
                                new RPolygon(
                                    triangle2,
                                    new Trianglef(color1, color3, color4),
                                    new Trianglef()
                                )
                            );
                        }
                        else {
                            //printPolyFlatZ(
                            //    new RPolygon(
                            //        triangle1,
                            //        new Trianglef(new Vec3f(0.671f, 0.345f, 0.745f), new Vec3f(0.671f, 0.345f, 0.745f), new Vec3f(0.671f, 0.345f, 0.745f)),
                            //        new Trianglef()
                            //    )
                            //);
                            //printPolyFlatZ(
                            //    new RPolygon(
                            //        triangle2,
                            //        new Trianglef(new Vec3f(0.671f, 0.345f, 0.745f), new Vec3f(0.671f, 0.345f, 0.745f), new Vec3f(0.671f, 0.345f, 0.745f)),
                            //        new Trianglef()
                            //    )
                            //);
                            printPolyFlatZ(
                                new RPolygon(
                                    triangle1,
                                    new Trianglef(new Vec3f(1.0f, 0.0f, 0.0f), new Vec3f(0.0f, 1.0f, 0.0f), new Vec3f(0.0f, 0.0f, 1.0f)),
                                    new Trianglef()
                                )
                            );
                            printPolyFlatZ(
                                new RPolygon(
                                    triangle2,
                                    new Trianglef(new Vec3f(0.671f, 0.345f, 0.745f), new Vec3f(0.671f, 0.345f, 0.745f), new Vec3f(0.671f, 0.345f, 0.745f)),
                                    new Trianglef()
                                )
                            );
                        }
                    //}
                }
            }
        }

        public void renderSolidColorZ(int frame) {

            Mat4f modelWorld = model * world;
            Mat4f proj = Mat4f.PerspectiveRH(GeneralVariables.M_PI / 3.0f, 320.0f / 200.0f, 0.1f, 1000.0f);

            foreach (Face face in loadResult.Groups[frame].Faces) {

                if (face.Count == 3) {

                    Vec4f point1Original = new Vec4f(
                        loadResult.Vertices[face[0].VertexIndex - 1].X,
                        loadResult.Vertices[face[0].VertexIndex - 1].Y,
                        loadResult.Vertices[face[0].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point1 = modelWorld * point1Original;
                    point1 = proj * point1;
                    point1.x = (point1.x + 1.0f) / 2.0f * 320.0f;
                    point1.y = (point1.y + 1.0f) / 2.0f * 200.0f;
                    //point1.z = point1Original.z;
                    Vec3f color1 = new Vec3f(
                        loadResult.Vertices[face[0].VertexIndex - 1].R,
                        loadResult.Vertices[face[0].VertexIndex - 1].G,
                        loadResult.Vertices[face[0].VertexIndex - 1].B
                    );


                    Vec4f point2Original = new Vec4f(
                        loadResult.Vertices[face[1].VertexIndex - 1].X,
                        loadResult.Vertices[face[1].VertexIndex - 1].Y,
                        loadResult.Vertices[face[1].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point2 = modelWorld * point2Original;
                    point2 = proj * point2;
                    point2.x = (point2.x + 1.0f) / 2.0f * 320.0f;
                    point2.y = (point2.y + 1.0f) / 2.0f * 200.0f;
                    //point2.z = point2Original.z;
                    Vec3f color2 = new Vec3f(
                        loadResult.Vertices[face[1].VertexIndex - 1].R,
                        loadResult.Vertices[face[1].VertexIndex - 1].G,
                        loadResult.Vertices[face[1].VertexIndex - 1].B
                    );

                    Vec4f point3Original = new Vec4f(
                        loadResult.Vertices[face[2].VertexIndex - 1].X,
                        loadResult.Vertices[face[2].VertexIndex - 1].Y,
                        loadResult.Vertices[face[2].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point3 = modelWorld * point3Original;
                    point3 = proj * point3;
                    point3.x = (point3.x + 1.0f) / 2.0f * 320.0f;
                    point3.y = (point3.y + 1.0f) / 2.0f * 200.0f;
                    //point3.z = point3Original.z;
                    Vec3f color3 = new Vec3f(
                        loadResult.Vertices[face[2].VertexIndex - 1].R,
                        loadResult.Vertices[face[2].VertexIndex - 1].G,
                        loadResult.Vertices[face[2].VertexIndex - 1].B
                    );

                    Trianglef triangle3Original = new Trianglef(point1Original, point2Original, point3Original);
                    Trianglef triangle3 = new Trianglef(point1, point2, point3);

                    Vec3f worldV = new Vec3f(modelWorld * triangle3Original.v0);
                    Trianglef triangle = new Trianglef(modelWorld * point1Original, modelWorld * point2Original, modelWorld * point3Original);
                    Vec3f N = triangle.tangent();
                    float cosTheta = worldV.dot(N);
                    bool visible = cosTheta < 0.0f;

                    Vec3f color = (color1 + color2 + color3) * (1.0f / 3.0f);

                    if (visible) {
                        if (color.x != 0.0f) {
                            printTriangleFlatZ(
                                triangle3,
                                color
                            );
                        }
                        else {
                            printTriangleFlatZ(
                                triangle3,
                                new Vec3f(0.671f, 0.345f, 0.745f)
                            );
                        }
                    }
                }
                else {

                    Vec4f point1Original = new Vec4f(
                        loadResult.Vertices[face[0].VertexIndex - 1].X,
                        loadResult.Vertices[face[0].VertexIndex - 1].Y,
                        loadResult.Vertices[face[0].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point1 = modelWorld * point1Original;
                    point1 = proj * point1;
                    point1.x = (point1.x + 1.0f) / 2.0f * 320.0f;
                    point1.y = (point1.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point2Original = new Vec4f(
                        loadResult.Vertices[face[1].VertexIndex - 1].X,
                        loadResult.Vertices[face[1].VertexIndex - 1].Y,
                        loadResult.Vertices[face[1].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point2 = modelWorld * point2Original;
                    point2 = proj * point2;
                    point2.x = (point2.x + 1.0f) / 2.0f * 320.0f;
                    point2.y = (point2.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point3Original = new Vec4f(
                        loadResult.Vertices[face[2].VertexIndex - 1].X,
                        loadResult.Vertices[face[2].VertexIndex - 1].Y,
                        loadResult.Vertices[face[2].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point3 = modelWorld * point3Original;
                    point3 = proj * point3;
                    point3.x = (point3.x + 1.0f) / 2.0f * 320.0f;
                    point3.y = (point3.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point4Original = new Vec4f(
                        loadResult.Vertices[face[3].VertexIndex - 1].X,
                        loadResult.Vertices[face[3].VertexIndex - 1].Y,
                        loadResult.Vertices[face[3].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point4 = modelWorld * point4Original;
                    point4 = proj * point4;
                    point4.x = (point4.x + 1.0f) / 2.0f * 320.0f;
                    point4.y = (point4.y + 1.0f) / 2.0f * 200.0f;

                    Trianglef triangle1Original = new Trianglef(point1Original, point2Original, point3Original);
                    Trianglef triangle2Original = new Trianglef(point1Original, point3Original, point4Original);
                    Trianglef triangle1 = new Trianglef(point1, point2, point3);
                    Trianglef triangle2 = new Trianglef(point1, point3, point4);

                    Vec3f worldV = new Vec3f(modelWorld * triangle1Original.v0);
                    Trianglef triangle = new Trianglef(modelWorld * point1Original, modelWorld * point2Original, modelWorld * point3Original);
                    Vec3f N = triangle.tangent();
                    float cosTheta = worldV.dot(N);
                    bool visible = cosTheta < 0.0f;

                    if (visible) {
                        printTriangleFlatZ(
                            triangle1,
                            new Vec3f(0.671f, 0.345f, 0.745f)
                        );

                        printTriangleFlatZ(
                            triangle2,
                            new Vec3f(0.671f, 0.345f, 0.745f)
                        );
                    }
                }
            }
        }

        public void renderSolidColor(int frame) {

            Mat4f modelWorld = model * world;
            Mat4f proj = Mat4f.PerspectiveRH(GeneralVariables.M_PI / 3.0f, 320.0f / 200.0f, 0.1f, 1000.0f);

            foreach (Face face in loadResult.Groups[frame].Faces) {

                if (face.Count == 3) {

                    Vec4f point1Original = new Vec4f(
                        loadResult.Vertices[face[0].VertexIndex - 1].X,
                        loadResult.Vertices[face[0].VertexIndex - 1].Y,
                        loadResult.Vertices[face[0].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point1 = modelWorld * point1Original;
                    point1 = proj * point1;
                    point1.x = (point1.x + 1.0f) / 2.0f * 320.0f;
                    point1.y = (point1.y + 1.0f) / 2.0f * 200.0f;
                    Vec3f color1 = new Vec3f(
                        loadResult.Vertices[face[0].VertexIndex - 1].R,
                        loadResult.Vertices[face[0].VertexIndex - 1].G,
                        loadResult.Vertices[face[0].VertexIndex - 1].B
                    );


                    Vec4f point2Original = new Vec4f(
                        loadResult.Vertices[face[1].VertexIndex - 1].X,
                        loadResult.Vertices[face[1].VertexIndex - 1].Y,
                        loadResult.Vertices[face[1].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point2 = modelWorld * point2Original;
                    point2 = proj * point2;
                    point2.x = (point2.x + 1.0f) / 2.0f * 320.0f;
                    point2.y = (point2.y + 1.0f) / 2.0f * 200.0f;
                    Vec3f color2 = new Vec3f(
                        loadResult.Vertices[face[1].VertexIndex - 1].R,
                        loadResult.Vertices[face[1].VertexIndex - 1].G,
                        loadResult.Vertices[face[1].VertexIndex - 1].B
                    );

                    Vec4f point3Original = new Vec4f(
                        loadResult.Vertices[face[2].VertexIndex - 1].X,
                        loadResult.Vertices[face[2].VertexIndex - 1].Y,
                        loadResult.Vertices[face[2].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point3 = modelWorld * point3Original;
                    point3 = proj * point3;
                    point3.x = (point3.x + 1.0f) / 2.0f * 320.0f;
                    point3.y = (point3.y + 1.0f) / 2.0f * 200.0f;
                    Vec3f color3 = new Vec3f(
                        loadResult.Vertices[face[2].VertexIndex - 1].R,
                        loadResult.Vertices[face[2].VertexIndex - 1].G,
                        loadResult.Vertices[face[2].VertexIndex - 1].B
                    );

                    Trianglef triangle3Original = new Trianglef(point1Original, point2Original, point3Original);
                    Trianglef triangle3 = new Trianglef(point1, point2, point3);

                    Vec3f worldV = new Vec3f(modelWorld * triangle3Original.v0);
                    Trianglef triangle = new Trianglef(modelWorld * point1Original, modelWorld * point2Original, modelWorld * point3Original);
                    Vec3f N = triangle.tangent();
                    float cosTheta = worldV.dot(N);
                    bool visible = cosTheta < 0.0f;

                    Vec3f color = (color1 + color2 + color3) * (1.0f / 3.0f);

                    if (visible) {
                        printTriangleFlat(
                            triangle3,
                            color
                        );
                    }

                }
                else {

                    Vec4f point1Original = new Vec4f(
                        loadResult.Vertices[face[0].VertexIndex - 1].X,
                        loadResult.Vertices[face[0].VertexIndex - 1].Y,
                        loadResult.Vertices[face[0].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point1 = modelWorld * point1Original;
                    point1 = proj * point1;
                    point1.x = (point1.x + 1.0f) / 2.0f * 320.0f;
                    point1.y = (point1.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point2Original = new Vec4f(
                        loadResult.Vertices[face[1].VertexIndex - 1].X,
                        loadResult.Vertices[face[1].VertexIndex - 1].Y,
                        loadResult.Vertices[face[1].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point2 = modelWorld * point2Original;
                    point2 = proj * point2;
                    point2.x = (point2.x + 1.0f) / 2.0f * 320.0f;
                    point2.y = (point2.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point3Original = new Vec4f(
                        loadResult.Vertices[face[2].VertexIndex - 1].X,
                        loadResult.Vertices[face[2].VertexIndex - 1].Y,
                        loadResult.Vertices[face[2].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point3 = modelWorld * point3Original;
                    point3 = proj * point3;
                    point3.x = (point3.x + 1.0f) / 2.0f * 320.0f;
                    point3.y = (point3.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point4Original = new Vec4f(
                        loadResult.Vertices[face[3].VertexIndex - 1].X,
                        loadResult.Vertices[face[3].VertexIndex - 1].Y,
                        loadResult.Vertices[face[3].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point4 = modelWorld * point4Original;
                    point4 = proj * point4;
                    point4.x = (point4.x + 1.0f) / 2.0f * 320.0f;
                    point4.y = (point4.y + 1.0f) / 2.0f * 200.0f;

                    Trianglef triangle1Original = new Trianglef(point1Original, point2Original, point3Original);
                    Trianglef triangle2Original = new Trianglef(point1Original, point3Original, point4Original);
                    Trianglef triangle1 = new Trianglef(point1, point2, point3);
                    Trianglef triangle2 = new Trianglef(point1, point3, point4);

                    Vec3f worldV = new Vec3f(modelWorld * triangle1Original.v0);
                    Trianglef triangle = new Trianglef(modelWorld * point1Original, modelWorld * point2Original, modelWorld * point3Original);
                    Vec3f N = triangle.tangent();
                    float cosTheta = worldV.dot(N);
                    bool visible = cosTheta < 0.0f;

                    if (visible) {
                        printTriangleFlat(
                            triangle1,
                            new Vec3f(0.671f, 0.345f, 0.745f)
                        );

                        printTriangleFlat(
                            triangle2,
                            new Vec3f(0.671f, 0.345f, 0.745f)
                        );
                    }
                }
            }
        }

        public void renderFlatWithNormals(int frame) {

            Mat4f modelWorld = model * world;
            Mat4f proj = Mat4f.PerspectiveRH(GeneralVariables.M_PI / 3.0f, 320.0f / 200.0f, 0.1f, 1000.0f);
            //Mat4f proj = Mat4f.ProjectionMatrix4(90.0f, 0.1f, 1000.0f);

            foreach (Face face in loadResult.Groups[frame].Faces) {

                if (face.Count == 3) {

                    Vec4f point1Original = new Vec4f(
                        loadResult.Vertices[face[0].VertexIndex - 1].X,
                        loadResult.Vertices[face[0].VertexIndex - 1].Y,
                        loadResult.Vertices[face[0].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point1 = modelWorld * point1Original;
                    point1 = proj * point1;
                    point1.x = (point1.x + 1.0f) / 2.0f * 320.0f;
                    point1.y = (point1.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point2Original = new Vec4f(
                        loadResult.Vertices[face[1].VertexIndex - 1].X,
                        loadResult.Vertices[face[1].VertexIndex - 1].Y,
                        loadResult.Vertices[face[1].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point2 = modelWorld * point2Original;
                    point2 = proj * point2;
                    point2.x = (point2.x + 1.0f) / 2.0f * 320.0f;
                    point2.y = (point2.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point3Original = new Vec4f(
                        loadResult.Vertices[face[2].VertexIndex - 1].X,
                        loadResult.Vertices[face[2].VertexIndex - 1].Y,
                        loadResult.Vertices[face[2].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point3 = modelWorld * point3Original;
                    point3 = proj * point3;
                    point3.x = (point3.x + 1.0f) / 2.0f * 320.0f;
                    point3.y = (point3.y + 1.0f) / 2.0f * 200.0f;

                    Trianglef triangle3Original = new Trianglef(point1Original, point2Original, point3Original);
                    Trianglef triangle3 = new Trianglef(point1, point2, point3);

                    Vec3f worldV = new Vec3f(modelWorld * triangle3Original.v0);
                    Trianglef triangle = new Trianglef(modelWorld * point1Original, modelWorld * point2Original, modelWorld * point3Original);
                    Vec3f N = triangle.tangent();
                    float cosTheta = worldV.dot(N);
                    bool visible = cosTheta < 0.0f;

                    if (visible) {
                        printTriangleFlat(
                            triangle3,
                            new Vec3f(0.671f, 0.345f, 0.745f)
                        );
                    }

                    Vec4f point4 = new Vec4f(triangle3Original.center(), 1.0f);
                    Vec4f point5 = point4 + new Vec4f(triangle3Original.normal(), 0.0f) * 0.125f;
                    point4 = modelWorld * point4;
                    point4 = proj * point4;
                    point4.x = (point4.x + 1.0f) / 2.0f * 320.0f;
                    point4.y = (point4.y + 1.0f) / 2.0f * 200.0f;
                    point5 = modelWorld * point5;
                    point5 = proj * point5;
                    point5.x = (point5.x + 1.0f) / 2.0f * 320.0f;
                    point5.y = (point5.y + 1.0f) / 2.0f * 200.0f;


                    if (visible) {
                        printLine(
                            new Linef(point4, point5),
                            new Vec3f(0.0f, 0.0f, 1.0f)
                        );
                    }
                }
                else {

                    Vec4f point1Original = new Vec4f(
                        loadResult.Vertices[face[0].VertexIndex - 1].X,
                        loadResult.Vertices[face[0].VertexIndex - 1].Y,
                        loadResult.Vertices[face[0].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point1 = modelWorld * point1Original;
                    point1 = proj * point1;
                    point1.x = (point1.x + 1.0f) / 2.0f * 320.0f;
                    point1.y = (point1.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point2Original = new Vec4f(
                        loadResult.Vertices[face[1].VertexIndex - 1].X,
                        loadResult.Vertices[face[1].VertexIndex - 1].Y,
                        loadResult.Vertices[face[1].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point2 = modelWorld * point2Original;
                    point2 = proj * point2;
                    point2.x = (point2.x + 1.0f) / 2.0f * 320.0f;
                    point2.y = (point2.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point3Original = new Vec4f(
                        loadResult.Vertices[face[2].VertexIndex - 1].X,
                        loadResult.Vertices[face[2].VertexIndex - 1].Y,
                        loadResult.Vertices[face[2].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point3 = modelWorld * point3Original;
                    point3 = proj * point3;
                    point3.x = (point3.x + 1.0f) / 2.0f * 320.0f;
                    point3.y = (point3.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point4Original = new Vec4f(
                        loadResult.Vertices[face[3].VertexIndex - 1].X,
                        loadResult.Vertices[face[3].VertexIndex - 1].Y,
                        loadResult.Vertices[face[3].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point4 = modelWorld * point4Original;
                    point4 = proj * point4;
                    point4.x = (point4.x + 1.0f) / 2.0f * 320.0f;
                    point4.y = (point4.y + 1.0f) / 2.0f * 200.0f;

                    Trianglef triangle1Original = new Trianglef(point1Original, point2Original, point3Original);
                    Trianglef triangle2Original = new Trianglef(point1Original, point3Original, point4Original);
                    Trianglef triangle1 = new Trianglef(point1, point2, point3);
                    Trianglef triangle2 = new Trianglef(point1, point3, point4);


                    Vec3f worldV = new Vec3f(modelWorld * triangle1Original.v0);
                    Trianglef triangle = new Trianglef(modelWorld * point1Original, modelWorld * point2Original, modelWorld * point3Original);
                    Vec3f N = triangle.tangent();
                    float cosTheta = worldV.dot(N);
                    bool visible = cosTheta < 0.0f;

                    if (visible) {
                        printTriangleFlat(
                            triangle1,
                            new Vec3f(0.671f, 0.345f, 0.745f)
                        );

                        printTriangleFlat(
                            triangle2,
                            new Vec3f(0.671f, 0.345f, 0.745f)
                        );
                    }


                    Vec4f point5 = new Vec4f(triangle1Original.center(), 1.0f);
                    Vec4f point6 = point5 + new Vec4f(triangle1Original.normal(), 0.0f) * 0.125f;
                    point5 = modelWorld * point5;
                    point5 = proj * point5;
                    point5.x = (point5.x + 1.0f) / 2.0f * 320.0f;
                    point5.y = (point5.y + 1.0f) / 2.0f * 200.0f;
                    point6 = modelWorld * point6;
                    point6 = proj * point6;
                    point6.x = (point6.x + 1.0f) / 2.0f * 320.0f;
                    point6.y = (point6.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point7 = new Vec4f(triangle2Original.center(), 1.0f);
                    Vec4f point8 = point7 + new Vec4f(triangle2Original.normal(), 0.0f) * 0.125f;
                    point7 = modelWorld * point7;
                    point7 = proj * point7;
                    point7.x = (point7.x + 1.0f) / 2.0f * 320.0f;
                    point7.y = (point7.y + 1.0f) / 2.0f * 200.0f;
                    point8 = modelWorld * point8;
                    point8 = proj * point8;
                    point8.x = (point8.x + 1.0f) / 2.0f * 320.0f;
                    point8.y = (point8.y + 1.0f) / 2.0f * 200.0f;


                    if (visible) {
                        printLine(
                            new Linef(point5, point6),
                            new Vec3f(1.0f, 0.0f, 0.0f)
                        );

                        printLine(
                            new Linef(point7, point8),
                            new Vec3f(0.0f, 1.0f, 0.0f)
                        );
                    }
                }
            }
        }


        public void renderWireFrameWithNormals(int frame) {

            Mat4f modelWorld = model * world;
            Mat4f proj = Mat4f.PerspectiveRH(GeneralVariables.M_PI / 3.0f, 320.0f / 200.0f, 0.1f, 1000.0f);

            foreach (Face face in loadResult.Groups[frame].Faces) {

                if (face.Count == 3) {

                    Vec4f point1Original = new Vec4f(
                        loadResult.Vertices[face[0].VertexIndex - 1].X,
                        loadResult.Vertices[face[0].VertexIndex - 1].Y,
                        loadResult.Vertices[face[0].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point1 = modelWorld * point1Original;
                    point1 = proj * point1;
                    point1.x = (point1.x + 1.0f) / 2.0f * 320.0f;
                    point1.y = (point1.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point2Original = new Vec4f(
                        loadResult.Vertices[face[1].VertexIndex - 1].X,
                        loadResult.Vertices[face[1].VertexIndex - 1].Y,
                        loadResult.Vertices[face[1].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point2 = modelWorld * point2Original;
                    point2 = proj * point2;
                    point2.x = (point2.x + 1.0f) / 2.0f * 320.0f;
                    point2.y = (point2.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point3Original = new Vec4f(
                        loadResult.Vertices[face[2].VertexIndex - 1].X,
                        loadResult.Vertices[face[2].VertexIndex - 1].Y,
                        loadResult.Vertices[face[2].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point3 = modelWorld * point3Original;
                    point3 = proj * point3;
                    point3.x = (point3.x + 1.0f) / 2.0f * 320.0f;
                    point3.y = (point3.y + 1.0f) / 2.0f * 200.0f;

                    Trianglef triangle3Original = new Trianglef(point1Original, point2Original, point3Original);
                    Trianglef triangle3 = new Trianglef(point1, point2, point3);

                    Vec3f worldV = new Vec3f(modelWorld * triangle3Original.v0);
                    Trianglef triangle = new Trianglef(modelWorld * point1Original, modelWorld * point2Original, modelWorld * point3Original);
                    Vec3f N = triangle.tangent();
                    float cosTheta = worldV.dot(N);
                    bool visible = cosTheta < 0.0f;

                    if (visible) {
                        printTriangleWireframe(
                            triangle3,
                            new Vec3f(0.255f, 0.368f, 0.765f)
                        );
                    }

                    Vec4f point4 = new Vec4f(triangle3Original.center(), 1.0f);
                    Vec4f point5 = point4 + new Vec4f(triangle3Original.normal(), 0.0f) * 0.125f;
                    point4 = modelWorld * point4;
                    point4 = proj * point4;
                    point4.x = (point4.x + 1.0f) / 2.0f * 320.0f;
                    point4.y = (point4.y + 1.0f) / 2.0f * 200.0f;
                    point5 = modelWorld * point5;
                    point5 = proj * point5;
                    point5.x = (point5.x + 1.0f) / 2.0f * 320.0f;
                    point5.y = (point5.y + 1.0f) / 2.0f * 200.0f;


                    if (visible) {
                        printLine(
                            new Linef(point4, point5),
                            new Vec3f(0.0f, 0.0f, 1.0f)
                        );
                    }
                }
                else {

                    Vec4f point1Original = new Vec4f(
                        loadResult.Vertices[face[0].VertexIndex - 1].X,
                        loadResult.Vertices[face[0].VertexIndex - 1].Y,
                        loadResult.Vertices[face[0].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point1 = modelWorld * point1Original;
                    point1 = proj * point1;
                    point1.x = (point1.x + 1.0f) / 2.0f * 320.0f;
                    point1.y = (point1.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point2Original = new Vec4f(
                        loadResult.Vertices[face[1].VertexIndex - 1].X,
                        loadResult.Vertices[face[1].VertexIndex - 1].Y,
                        loadResult.Vertices[face[1].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point2 = modelWorld * point2Original;
                    point2 = proj * point2;
                    point2.x = (point2.x + 1.0f) / 2.0f * 320.0f;
                    point2.y = (point2.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point3Original = new Vec4f(
                        loadResult.Vertices[face[2].VertexIndex - 1].X,
                        loadResult.Vertices[face[2].VertexIndex - 1].Y,
                        loadResult.Vertices[face[2].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point3 = modelWorld * point3Original;
                    point3 = proj * point3;
                    point3.x = (point3.x + 1.0f) / 2.0f * 320.0f;
                    point3.y = (point3.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point4Original = new Vec4f(
                        loadResult.Vertices[face[3].VertexIndex - 1].X,
                        loadResult.Vertices[face[3].VertexIndex - 1].Y,
                        loadResult.Vertices[face[3].VertexIndex - 1].Z,
                        1.0f
                    );
                    Vec4f point4 = modelWorld * point4Original;
                    point4 = proj * point4;
                    point4.x = (point4.x + 1.0f) / 2.0f * 320.0f;
                    point4.y = (point4.y + 1.0f) / 2.0f * 200.0f;

                    Trianglef triangle1Original = new Trianglef(point1Original, point2Original, point3Original);
                    Trianglef triangle2Original = new Trianglef(point1Original, point3Original, point4Original);
                    Trianglef triangle1 = new Trianglef(point1, point2, point3);
                    Trianglef triangle2 = new Trianglef(point1, point3, point4);


                    Vec3f worldV = new Vec3f(modelWorld * triangle1Original.v0);
                    Trianglef triangle = new Trianglef(modelWorld * point1Original, modelWorld * point2Original, modelWorld * point3Original);
                    Vec3f N = triangle.tangent();
                    float cosTheta = worldV.dot(N);
                    bool visible = cosTheta < 0.0f;

                    if (visible) {
                        printTriangleWireframe(
                            triangle1,
                            new Vec3f(0.255f, 0.368f, 0.765f)
                        );

                        printTriangleWireframe(
                            triangle2,
                            new Vec3f(0.255f, 0.368f, 0.765f)
                        );
                    }


                    Vec4f point5 = new Vec4f(triangle1Original.center(), 1.0f);
                    Vec4f point6 = point5 + new Vec4f(triangle1Original.normal(), 0.0f) * 0.125f;
                    point5 = modelWorld * point5;
                    point5 = proj * point5;
                    point5.x = (point5.x + 1.0f) / 2.0f * 320.0f;
                    point5.y = (point5.y + 1.0f) / 2.0f * 200.0f;
                    point6 = modelWorld * point6;
                    point6 = proj * point6;
                    point6.x = (point6.x + 1.0f) / 2.0f * 320.0f;
                    point6.y = (point6.y + 1.0f) / 2.0f * 200.0f;

                    Vec4f point7 = new Vec4f(triangle2Original.center(), 1.0f);
                    Vec4f point8 = point7 + new Vec4f(triangle2Original.normal(), 0.0f) * 0.125f;
                    point7 = modelWorld * point7;
                    point7 = proj * point7;
                    point7.x = (point7.x + 1.0f) / 2.0f * 320.0f;
                    point7.y = (point7.y + 1.0f) / 2.0f * 200.0f;
                    point8 = modelWorld * point8;
                    point8 = proj * point8;
                    point8.x = (point8.x + 1.0f) / 2.0f * 320.0f;
                    point8.y = (point8.y + 1.0f) / 2.0f * 200.0f;


                    if (visible) {
                        printLine(
                            new Linef(point5, point6),
                            new Vec3f(1.0f, 0.0f, 0.0f)
                        );

                        printLine(
                            new Linef(point7, point8),
                            new Vec3f(0.0f, 1.0f, 0.0f)
                        );
                    }
                }
            }
        }
    }
}
