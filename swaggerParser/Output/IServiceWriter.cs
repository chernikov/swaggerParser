using swaggerParser.Output.Base;
using swaggerParser.Output.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output
{
    public interface IServiceWriter
    {
        List<ServiceFile> GenerateFiles(List<BaseService> services);
    }
}
