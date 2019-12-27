using ObjReader.Data.VertexData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjReader.Data.DataStore {

    public interface ITextureDataStore {

        void AddTexture(Texture texture);
    }
}
