using swaggerParser.Output.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output.Typescript
{
    public class TypescriptService : BaseService
    {
        public new List<TypescriptAction> Actions { get; set; }

        public TypescriptService(BaseService @base)
        {
            Name = @base.Name;
            Actions = @base.Actions.Select(p => new TypescriptAction(p)).ToList();
        }
    }
}
