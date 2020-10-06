using swaggerParser.Output.Base;
using swaggerParser.Output.Enums;
using swaggerParser.Swagger;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace swaggerParser.Output.Typescript
{
    [DebuggerDisplay("Class: {FullName}")]
    public class TypescriptClass : BaseClass, ITypescriptType
    {
        public TypescriptClass()  { }

        public TypescriptClass (BaseClass @base) : base(@base) { }


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
                    return $"{{ [id: string]: {new TypescriptClass(new BaseClass(InnerClass)).AngularType}; }}";
                }
                switch (Type)
                {
                    case ClassTypeEnum.Array:
                        return (new TypescriptClass(new BaseClass(InnerClass))).AngularType + "[]";
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
    }
}
