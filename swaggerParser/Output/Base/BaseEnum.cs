using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output.Base
{
    [DebuggerDisplay("Enum: {FullName}")]
    public class BaseEnum : BaseType
    {
        public BaseEnum()
        {

        }

        public BaseEnum(BaseType @base)
        {
            Name = @base.Name;
            IsDictionary = @base.IsDictionary;
            InnerClass = @base.InnerClass;
            Type = @base.Type;
        }
        public BaseEnum(BaseEnum @base) : this(@base as BaseType)
        {
            Types = @base.Types;
        }

        public List<object> Types { get; set; }

        public override string FullName => Name;
    }
}
