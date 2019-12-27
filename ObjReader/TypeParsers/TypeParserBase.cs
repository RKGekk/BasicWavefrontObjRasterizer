using ObjReader.Common;
using ObjReader.TypeParsers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjReader.TypeParsers {

    public abstract class TypeParserBase : ITypeParser {

        protected abstract string Keyword { get; }

        public bool CanParse(string keyword) {
            return keyword.EqualsOrdinalIgnoreCase(Keyword);
        }

        public abstract void Parse(string line);
    }
}
