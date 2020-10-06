using swaggerParser.Output.Base;
using swaggerParser.Output.Enums;
using swaggerParser.Swagger;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace swaggerParser.Output.Typescript
{
    [DebuggerDisplay("Enum: {FullName}")]
    public class TypescriptEnum : BaseEnum, ITypescriptType
    {
        public TypescriptEnum() { }

        public TypescriptEnum(BaseEnum @base) : base(@base) { }

        public string AngularName
        {
            get
            {
                if (Name != null && Name.IndexOf("Dto") != -1)
                {
                    var nameProp = Name.Replace("Dto", "");
                    return nameProp;
                }
                return Name;
            }
        }

        public string AngularType
        {
            get
            {
                if (Name != null)
                {
                    return AngularName;
                }
                if (IsDictionary)
                {
                    return $"{{ [id: string]: {((ITypescriptType)InnerClass).AngularType}; }}";
                }
                switch (Type)
                {
                    case ClassTypeEnum.Array:
                        return ((ITypescriptType)InnerClass).AngularType + "[]";
                    case ClassTypeEnum.Object:
                        return Name;
                    case ClassTypeEnum.Boolean:
                        return "boolean";
                    case ClassTypeEnum.Byte:
                    case ClassTypeEnum.Integer:
                    case ClassTypeEnum.Float:
                    case ClassTypeEnum.Double:
                    case ClassTypeEnum.Long:
                        return "number";
                    case ClassTypeEnum.String:
                    case ClassTypeEnum.DateTime:
                        return "string";
                    default:
                        return $"Type: {Type}";
                }

            }
        }

        public string DefaultValue
        {
            get
            {
                if (Name != null)
                {
                    return "null";
                }
                if (IsDictionary)
                {
                    return "null";
                }
                switch (Type)
                {
                    case ClassTypeEnum.Array:
                        return "[]";
                    case ClassTypeEnum.Object:
                        return "null";
                    case ClassTypeEnum.Boolean:
                        return "false";
                    case ClassTypeEnum.Byte:
                    case ClassTypeEnum.Integer:
                    case ClassTypeEnum.Float:
                    case ClassTypeEnum.Double:
                    case ClassTypeEnum.Long:
                        return "0";
                    case ClassTypeEnum.String:
                    case ClassTypeEnum.DateTime:
                        return "''";
                    default:
                        return "null";
                }
            }
        }

        public List<BaseProperty> Properties => throw new System.NotImplementedException();
    }
}
