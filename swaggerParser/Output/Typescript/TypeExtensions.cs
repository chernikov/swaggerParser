using swaggerParser.Output.Base;
using swaggerParser.Output.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace swaggerParser.Output.Typescript
{
    public static class TypeExtensions
    {
        public static string GetKebabName(this BaseService service)
        {
            return GetKebab(service.Name);
        }

        public static string GetKebabName(this BaseType @class)
        {
            return GetKebab(@class.GetTypescriptName());
        }


        public static string GetTypescriptName(this BaseType @class)
        {
            var name = @class.Name;
            if (name != null && name.IndexOf("Dto") != -1)
            {
                var nameProp = name.Replace("Dto", "");
                return nameProp;
            }
            return name;
        }

        public static string GetTypescriptType(this BaseType @class)
        {
            if (@class.Name != null)
            {
                return GetTypescriptName(@class);
            }
            if (@class.IsDictionary)
            {
                return $"{{ [id: string]: {@class.InnerClass.GetTypescriptType()}; }}";
            }
            switch (@class.Type)
            {
                case ClassTypeEnum.Array:
                    return @class.InnerClass.GetTypescriptType() + "[]";
                case ClassTypeEnum.Object:
                    return @class.Name;
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
                    return $"Type: {@class.Type}";
            }
        }

        public static string GetDefaultValue(this BaseType @class)
        {
            if (@class.Name != null)
            {
                return "null";
            }
            if (@class.IsDictionary)
            {
                return "null";
            }
            switch (@class.Type)
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


        private static string GetKebab(string @src)
        {
            if (string.IsNullOrEmpty(@src))
            {
                return @src;
            }

            return Regex.Replace(@src, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "-$1", RegexOptions.Compiled).Trim().ToLower();
        }
    }
}
