using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjReader.Loaders {

    public interface IObjLoaderFactory {

        IObjLoader Create(IMaterialStreamProvider materialStreamProvider);
        IObjLoader Create();
    }
}
