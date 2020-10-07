using swaggerParser.Output.Base;
using swaggerParser.Output.Enums;
using swaggerParser.Swagger;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace swaggerParser.Output.Typescript
{
    [DebuggerDisplay("Class: {FullName}")]
    public class TypescriptClass : BaseClass
    {
        public override string FullName
        {
            get
            {
                switch (Type)
                {
                    case ClassTypeEnum.Array:
                        return $"{InnerClass.FullName}[]";
                    case ClassTypeEnum.Object:
                        if (IsDictionary)
                        {
                            return $"{InnerClass.FullName}[]";
                        }
                        return Name;
                    case ClassTypeEnum.Byte:
                        return "byte";
                    case ClassTypeEnum.DateTime:
                        return "DateTime";
                    case ClassTypeEnum.Double:
                        return "Double";
                    case ClassTypeEnum.Float:
                        return "Float";
                    case ClassTypeEnum.Integer:
                        return "Integer";
                    case ClassTypeEnum.Long:
                        return "Long";
                    case ClassTypeEnum.String:
                        return "String";
                }
                return Name;
            }
        }
    }
}
