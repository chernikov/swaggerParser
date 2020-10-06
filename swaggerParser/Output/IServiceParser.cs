using swaggerParser.Output.Base;
using swaggerParser.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output
{
    public interface IServiceParser
    {
        List<BaseService> GetServices(Document document, List<BaseType> baseOutputClasses);

    }
}
