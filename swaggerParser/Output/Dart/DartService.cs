using swaggerParser.Output.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output.Dart
{
    public class DartService : BaseService
    {
        public DartService(BaseService @base)
        {
            Name = @base.Name;
            Actions = @base.Actions.Select(p => (BaseAction)new DartAction(p)).ToList();
        }
    
        
    }
}
