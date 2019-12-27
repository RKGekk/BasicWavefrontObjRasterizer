using BasicRender.Engine;
using MathematicalEntities;
using ObjReader.Data.DataStore;
using ObjReader.Data.Elements;
using ObjReader.Loaders;
using ObjReader.TypeParsers;
using PhysicsEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BasicRender {

    public partial class MainWindow : Window {

        private GameTimer _timer = new GameTimer();

        private WriteableBitmap _wbStat;
        private Int32Rect _rectStat;
        private Renderer _rendererStat;

        private WriteableBitmap _wb;
        private Int32Rect _rect;
        private Renderer _renderer;

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e) {

            _timer.reset();
            _timer.start();

            int pixelWidth = (int)img.Width;
            int pixelHeight = (int)img.Height;

            _wb = new WriteableBitmap(pixelWidth, pixelHeight, 96, 96, PixelFormats.Bgra32, null);
            _rect = new Int32Rect(0, 0, pixelWidth, pixelHeight);

            byte[] pixels = new byte[pixelWidth * pixelHeight * _wb.Format.BitsPerPixel / 8];
            float[] zbuf = new float[pixelWidth * pixelHeight];
            _renderer = new Renderer(_timer, pixels, zbuf, pixelWidth);
            _renderer.fillScreen(new Vec3f(0.5f, 0.5f, 0.5f));
            _renderer.fillZBuff(1.0f / 1000.0f);
            //_renderer.setObject(ObjLoaderFactory.Create().Load(File.Open("untitled1.obj", FileMode.Open, FileAccess.Read, FileShare.None)));
            //_renderer.setObject(ObjLoaderFactory.Create().Load(File.Open("untitled3.obj", FileMode.Open, FileAccess.Read, FileShare.None)));
            //_renderer.setObject(ObjLoaderFactory.Create().Load(File.Open("sp2_648.mdl_a2.00.obj", FileMode.Open, FileAccess.Read, FileShare.None)));
            _renderer.setObject("sp2_648.mdl_a2.00.obj");

            //_renderer.setModel(
            //    new Mat4f(
            //        new Vec4f(1.0f, 0.0f, 0.0f, 0.0f),
            //        new Vec4f(0.0f, 1.0f, 0.0f, 0.0f),
            //        new Vec4f(0.0f, 0.0f, 1.0f, 0.0f),
            //        new Vec4f(0.0f, 0.0f, 0.0f, 1.0f)
            //    )
            //);
            _renderer.setModel(
                Mat4f.RotationXMatrix(-GeneralVariables.M_PI / 2.0f) * new Mat4f(
                    new Vec4f(0.005f, 0.0f, 0.0f, 0.0f),
                    new Vec4f(0.0f, -0.005f, 0.0f, 0.0f),
                    new Vec4f(0.0f, 0.0f, 0.005f, 0.0f),
                    new Vec4f(0.0f, 0.0f, 0.0f, 1.0f)
                )
            );
            //_renderer.setWorld(
            //    new Mat4f(
            //        new Vec4f(1.0f, 0.0f, 0.0f, 0.0f),
            //        new Vec4f(0.0f, 1.0f, 0.0f, 0.0f),
            //        new Vec4f(0.0f, 0.0f, 1.0f, 0.0f),
            //        new Vec4f(0.0f, -0.75f, 4.5f, 1.0f)
            //    )
            //);
            _renderer.setWorld(
                new Mat4f(
                    new Vec4f(1.0f, 0.0f, 0.0f, 0.0f),
                    new Vec4f(0.0f, 1.0f, 0.0f, 0.0f),
                    new Vec4f(0.0f, 0.0f, 1.0f, 0.0f),
                    new Vec4f(0.0f, 0.4f, 5.5f, 1.0f)
                )
            );
            //_renderer.setWorld(
            //    new Mat4f(
            //        new Vec4f(1.0f, 0.0f, 0.0f, 0.0f),
            //        new Vec4f(0.0f, 1.0f, 0.0f, 0.0f),
            //        new Vec4f(0.0f, 0.0f, 1.0f, 0.0f),
            //        new Vec4f(0.0f, 0.0f, 2.2f, 1.0f)
            //    )
            //);
            //_renderer.setWorld(
            //    new Mat4f(
            //        new Vec4f(1.0f, 0.0f, 0.0f, 0.0f),
            //        new Vec4f(0.0f, 1.0f, 0.0f, 0.0f),
            //        new Vec4f(0.0f, 0.0f, 1.0f, 0.0f),
            //        new Vec4f(0.0f, 0.4f, 6.5f, 1.0f)
            //    )
            //);

            //_renderer.setWorld(
            //    new Mat4f(
            //        new Vec4f(1.0f, 0.0f, 0.0f, 0.0f),
            //        new Vec4f(0.0f, 1.0f, 0.0f, 0.0f),
            //        new Vec4f(0.0f, 0.0f, 1.0f, 0.0f),
            //        new Vec4f(0.0f, 0.0f, 2.0f, 1.0f)
            //    )
            //);

            _wb.WritePixels(_rect, pixels, _renderer.stride, 0);

            img.Source = _wb;

            InitializeStats();

            CompositionTarget.Rendering += UpdateChildren;
        }

        private void InitializeStats() {

            int pixelWidthStat = (int)statImg.Width;
            int pixelHeightStat = (int)statImg.Height;

            _wbStat = new WriteableBitmap(pixelWidthStat, pixelHeightStat, 96, 96, PixelFormats.Bgra32, null);
            _rectStat = new Int32Rect(0, 0, pixelWidthStat, pixelHeightStat);

            byte[] pixelsStat = new byte[pixelWidthStat * pixelHeightStat * _wbStat.Format.BitsPerPixel / 8];

            _rendererStat = new Renderer(_timer, pixelsStat, null, pixelWidthStat);
            _rendererStat.fillScreen(new Vec3f(0.125f, 0.125f, 0.125f));

            _wbStat.WritePixels(_rectStat, pixelsStat, _rendererStat.stride, 0);

            statImg.Source = _wbStat;
        }

        private float _tt = 0.0f;
        private float _angle = 0.0f;
        private int _frame = 0;

        protected void UpdateChildren(object sender, EventArgs e) {

            RenderingEventArgs renderingArgs = e as RenderingEventArgs;
            _timer.tick();

            _renderer.fillScreen(new Vec3f(0.5f, 0.5f, 0.5f));
            _renderer.fillZBuff(1.0f);

            float duration = _timer.deltaTime();

            _tt += duration;
            if (_tt > (1.0f / 24.0f)) {
                _tt = 0.0f;
                _frame += 1;
                if (_frame >= _renderer.countFrames())
                    _frame = 0;
            }

            //model = model * Mat4f.RotationXMatrix(_angle);
            //model = model * Mat4f.RotationYMatrix(_angle);
            //model = model * Mat4f.RotationZMatrix(_angle);
            //model = model * Mat4f.RotationXMatrix(GeneralVariables.M_PI / 2.0f);
            //model = model * Mat4f.RotationYMatrix(M_PI / 2.0f);
            //model = model * Mat4f.RotationZMatrix(_angle);
            _renderer.setModel(Mat4f.RotationXMatrix(-GeneralVariables.M_PI / 2.0f) * Mat4f.RotationYMatrix(_angle) * new Mat4f(
                    new Vec4f(0.005f, 0.0f, 0.0f, 0.0f),
                    new Vec4f(0.0f, -0.005f, 0.0f, 0.0f),
                    new Vec4f(0.0f, 0.0f, 0.005f, 0.0f),
                    new Vec4f(0.0f, 0.0f, 0.0f, 1.0f)
                ));
            //_renderer.setModel(Mat4f.RotationXMatrix(-GeneralVariables.M_PI / 2.0f) * new Mat4f(
            //        new Vec4f(0.005f, 0.0f, 0.0f, 0.0f),
            //        new Vec4f(0.0f, -0.005f, 0.0f, 0.0f),
            //        new Vec4f(0.0f, 0.0f, 0.005f, 0.0f),
            //        new Vec4f(0.0f, 0.0f, 0.0f, 1.0f)
            //    ));
            //_renderer.setModel(new Mat4f(
            //        new Vec4f(1.0f, 0.0f, 0.0f, 0.0f),
            //        new Vec4f(0.0f, 1.0f, 0.0f, 0.0f),
            //        new Vec4f(0.0f, 0.0f, 1.0f, 0.0f),
            //        new Vec4f(0.0f, 0.0f, 0.0f, 1.0f)
            //    ));
            //_renderer.setModel(Mat4f.RotationYMatrix(_angle));
            //_renderer.setModel(Mat4f.RotationZMatrix(_angle) * Mat4f.RotationXMatrix(_angle));

            //_renderer.renderFlatWithNormals(_frame);
            //_renderer.renderWireFrameWithNormals(0);
            //_renderer.renderSolidColor(_frame);
            //_renderer.renderSolidColorZ(_frame);
            _renderer.renderPolySolidColorZ(_frame);
            //_renderer.renderWireFrameWithNormals(_frame);

            _angle += (float)(Math.PI / 128.0f);

            _wb.WritePixels(_rect, _renderer.buf, _renderer.stride, 0);

            updateStats();
        }

        private void updateStats() {

            float duration = _timer.deltaTime();
            float totalTime = _timer.gameTime();
            int iduration = (int)(duration * 1000.0f);

            statsText.Text = $"RenderDuration: {duration * 1000.0f:F2}ms; FPS: {1.0f / duration:F0}; TotalTime: {totalTime:F3}sec";

            _rendererStat.lmoveScreen(new Vec3f(0.125f, 0.125f, 0.125f), 1);
            
            if (iduration < 32)
                _rendererStat.printPixel(319, iduration, new Vec3f(0.0f, 0.5f, 0.0f));
            _wbStat.WritePixels(_rectStat, _rendererStat.buf, _rendererStat.stride, 0);
        }
    }
}
