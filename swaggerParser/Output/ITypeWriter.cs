using swaggerParser.Output.Base;
using swaggerParser.Output.Files;
using swaggerParser.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output
{
    public interface ITypeWriter
    {
        List<BaseFile> GenerateFiles(List<BaseType> types);
    }
}
