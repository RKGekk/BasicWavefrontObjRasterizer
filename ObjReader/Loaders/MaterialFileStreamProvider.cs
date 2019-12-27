using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjReader.Loaders {

    public class MaterialFileStreamProvider : IMaterialStreamProvider {

        public string RequestedMaterialFilePath { get; private set; }
        public Stream DataStream { get; set; }

        public Stream Open(string materialFilePath) {
            RequestedMaterialFilePath = materialFilePath;
            DataStream = new FileStream(materialFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return DataStream;
        }
    }
}
