using swaggerParser.Output.Base;
using swaggerParser.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output.Parsers
{
    public interface ITypeParser
    {
        List<BaseType> GetTypes(Document document);

        BaseType CreateDefinition(string name, DocumentSchema schema);

        BaseType GetClassDefinition(DocumentSchema schema, List<BaseType> baseOutputClasses = null);

        BaseType CreateDefinitionWithDeep(DocumentSchema schema, List<BaseType> list);
    }
}
