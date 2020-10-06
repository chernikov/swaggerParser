using swaggerParser.Output.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output.Typescript
{
    public interface ITypescriptType
    {
        string AngularName { get; }

        string AngularType { get; }

        string DefaultValue { get; }

        List<BaseProperty> Properties { get; }
    }
}
