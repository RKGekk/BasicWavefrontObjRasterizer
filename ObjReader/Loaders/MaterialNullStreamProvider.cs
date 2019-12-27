using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjReader.Loaders {

    public class MaterialNullStreamProvider : IMaterialStreamProvider {

        public Stream Open(string materialFilePath) {
            return null;
        }
    }
}
