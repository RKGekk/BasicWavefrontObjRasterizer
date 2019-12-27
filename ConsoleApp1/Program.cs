using MathematicalEntities;
using ObjReader.Data.DataStore;
using ObjReader.Loaders;
using ObjReader.TypeParsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1 {

    class Program {

        private class MaterialStreamProviderSpy : IMaterialStreamProvider {

            public string RequestedMaterialFilePath { get; private set; }
            public Stream StreamToReturn { get; set; }

            public Stream Open(string materialFilePath) {
                RequestedMaterialFilePath = materialFilePath;
                return StreamToReturn;
            }
        }
        
        static void Main(string[] args) {

            //Vec2f v0 = new Vec2f(209.0f, 87.0f);
            //Vec2f v1 = new Vec2f(110.0f, 87.0f);
            //Vec2f v2 = new Vec2f(209.0f, 186.0f);

            //float ff = Trianglef.edgeFunction(v0, v1, v2);

            Vec2f v0 = new Vec2f(125.0f, 22.0f);
            Vec2f v1 = new Vec2f(82.0f, 65.0f);
            Vec2f v2 = new Vec2f(151.0f, 91.0f);

            Trianglef tt = new Trianglef(v0, v1, v2);
            bool clock1 = tt.clockWise();

            v0 = new Vec2f(125.0f, 22.0f);
            v1 = new Vec2f(151.0f, 65.0f);
            v2 = new Vec2f(151.0f, 91.0f);

            tt = new Trianglef(v0, v1, v2);
            bool clock2 = tt.clockWise();

            string fs = "";

            //Vec3f L = new Vec3f(1.0f, -1.0f, 0.0f);
            //L.normalize();
            //Vec3f N = new Vec3f(0.0f, 1.0f, 0.0f);
            //Vec3f R = L.reflect(N);
            //float l1 = R.len();
            //Vec3f Rf = L.refract(N, 1.0f / 1.333f);
            //float l2 = Rf.len();

            /*
            string objPath = "untitled.obj";
            FileStream objfs = File.Open(objPath, FileMode.Open, FileAccess.Read, FileShare.None);

            //string mtlPath = "untitled.mtl";
            //FileStream mtlfs = File.Open(mtlPath, FileMode.Open, FileAccess.Read, FileShare.None);
            
            DataStore dataStore = new DataStore();
            FaceParser faceParser = new FaceParser(dataStore);
            GroupParser groupParser = new GroupParser(dataStore);
            NormalParser normalParser = new NormalParser(dataStore);
            TextureParser textureParser = new TextureParser(dataStore);
            VertexParser vertexParser = new VertexParser(dataStore);

            MaterialStreamProvider materialFileStreamProvider = new MaterialStreamProvider();
            MaterialLibraryLoader materialLibraryLoader = new MaterialLibraryLoader(dataStore);
            MaterialLibraryLoaderFacade materialLibraryLoaderFacade = new MaterialLibraryLoaderFacade(materialLibraryLoader, materialFileStreamProvider);
            MaterialLibraryParser materialLibraryParser = new MaterialLibraryParser(materialLibraryLoaderFacade);
            UseMaterialParser useMaterialParser = new UseMaterialParser(dataStore);

            ObjLoader objLoader = new ObjLoader(dataStore, faceParser, groupParser, normalParser, textureParser, vertexParser, materialLibraryParser, useMaterialParser);

            var objectStream = objfs;
            LoadResult loadResult = objLoader.Load(objectStream);
            */

            Console.ReadLine();
        }
    }
}
