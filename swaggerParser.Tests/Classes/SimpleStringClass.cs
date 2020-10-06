using swaggerParser.Output;
using swaggerParser.Output.Base;
using swaggerParser.Output.Enums;
using swaggerParser.Output.Typescript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Tests.Classes
{
    public class SimpleStringClass
    {
        public static BaseType ClassInstance => new BaseClass()
        {
            Name = "SimpleStringDto",
            Type = ClassTypeEnum.Object,
            Properties = new List<BaseProperty>()
            {
                new BaseProperty()
                {
                    Name = "Id",
                    Type = new BaseClass()
                    {
                        Type = ClassTypeEnum.String,
                    }
                }
            }
        };
    }
}
