using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjReader.Data.DataStore {

    public interface IGroupDataStore {

        void PushGroup(string groupName);
    }
}
