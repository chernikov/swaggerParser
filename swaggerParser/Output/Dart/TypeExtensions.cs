using swaggerParser.Output.Base;
using swaggerParser.Output.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace swaggerParser.Output.Dart
{
    public static class TypeExtensions
    {
        private static readonly List<string> DartKeywords = new List<string> { "final" };

        public static string GetSnakeName(this BaseService service)
        {
            return GetSnake(service.Name);
        }

        public static string GetSnakeName(this BaseType @class)
        {
            return GetSnake(@class.GetDartName());
        }


        public static string GetDartName(this BaseType @class)
        {
            var name = @class.Name;
            if (name != null && name.IndexOf("Dto") != -1)
            {
                var nameProp = name.Replace("Dto", "");
                return nameProp;
            }
            return name;
        }

        public static string GetDartType(this BaseType @class)
        {
            if (@class.Name != null)
            {
                return GetDartName(@class);
            }
            if (@class.IsDictionary)
            {
                return $"{{ [id: string]: {@class.InnerClass.GetDartType()}; }}";
            }
            switch (@class.Type)
            {
                case ClassTypeEnum.Array:
                    return $"List<{@class.InnerClass.GetDartType()}>";
                case ClassTypeEnum.Object:
                    return @class.Name;
                case ClassTypeEnum.Boolean:
                    return "bool";
                case ClassTypeEnum.Byte:
                case ClassTypeEnum.Integer:
                case ClassTypeEnum.Long:
                    return "int";
                case ClassTypeEnum.Float:
                case ClassTypeEnum.Double:
                    return "double";
                case ClassTypeEnum.DateTime:
                    return "DateTime";
                case ClassTypeEnum.String:
                    return "String";
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
                    return $"[]"; 
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
                    return "''";
                case ClassTypeEnum.DateTime:
                    return "null";
                default:
                    return "null";
            }
        }

        public static string GetDefaultValueOnlyValued(this BaseType @class)
        {
            if (@class.Name != null)
            {
                return null;
            }
            if (@class.IsDictionary)
            {
                return null;
            }
            switch (@class.Type)
            {
                case ClassTypeEnum.Array:
                    return null;
                case ClassTypeEnum.Object:
                    return null;
                case ClassTypeEnum.Boolean:
                    return "false";
                case ClassTypeEnum.Byte:
                case ClassTypeEnum.Integer:
                case ClassTypeEnum.Float:
                case ClassTypeEnum.Double:
                case ClassTypeEnum.Long:
                    return "0";
                case ClassTypeEnum.String:
                    return null;
                case ClassTypeEnum.DateTime:
                    return null;
                default:
                    return null;
            }
        }

        public static string AvoidKeywords(this string str)
        {
            if (DartKeywords.Contains(str))
            {
                return $"{str}_";
            }
            return str;
        }

        private static string GetSnake(string @src)
        {
            if (string.IsNullOrEmpty(@src))
            {
                return @src;
            }

            return Regex.Replace(@src, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "_$1", RegexOptions.Compiled).Trim().ToLower();
        }
    }
}
