using swaggerParser.Output.Base;
using swaggerParser.Output.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output.Dart
{
    public class DartTypeWriter : ITypeWriter
    {
        public List<BaseFile> GenerateFiles(List<BaseType> types)
        {
            var list = new List<BaseFile>();
            foreach (var _type in types)
            {
                BaseFile file = null;
                if (_type is BaseEnum)
                {
                    file = new EnumFile()
                    {
                        FileName = $"{_type.GetSnakeName()}.enum.dart",
                        Content = GenerateContentEnum(_type as BaseEnum)
                    };
                }
                if (_type is BaseClass)
                {
                    file = new ClassFile()
                    {
                        FileName = $"{_type.GetSnakeName()}.class.dart",
                        Content = GenerateContentClass(_type as BaseClass)
                    };
                }
                if (file != null)
                {
                    Console.WriteLine($"Generate file: {file.FileName}");
                    list.Add(file);
                }
            }
            return list;
        }

        private string GenerateContentEnum(BaseEnum @enum)
        {
            var sb = new StringBuilder();

            if (@enum.Types.Count > 0)
            {
                sb.AppendLine(GenerateEnum(@enum));
                sb.AppendLine(GenerateValueOfEnum(@enum));
                sb.AppendLine(GenerateEnumOfValue(@enum));
            }
            return sb.ToString();
        }



        private string GenerateEnum(BaseEnum @enum)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"enum {@enum.Name} {{");
            foreach (var type in @enum.Types)
            {
                if (type is int || type is long)
                {
                    int.TryParse(type.ToString(), out int result);
                    if (result < 0)
                    {
                        sb.AppendLine($"\tnumber_minus_{-result},");
                    }
                    else
                    {
                        sb.AppendLine($"\tnumber_{result},");
                    }
                }
                else
                {
                    sb.AppendLine($"\t{type}, ");
                }
            }
            sb.AppendLine($"}}");
            return sb.ToString();
        }

        private string GenerateValueOfEnum(BaseEnum @enum)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{@enum.Name} {@enum.Name}ValueOf(int value)  {{");
            sb.AppendLine($"  switch (value) {{");

            var firstType = @enum.Types[0];
            foreach (var type in @enum.Types)
            {
                if (type is int || type is long)
                {
                    int.TryParse(type.ToString(), out int result);
                    sb.AppendLine($"    case {result}:");
                    if (result < 0)
                    {
                        sb.AppendLine($"        return {@enum.Name}.number_minus_{-result};");
                    }
                    else
                    {
                        sb.AppendLine($"        return {@enum.Name}.number_{result};");
                    }
                }
            }
            sb.AppendLine("  }");
            int.TryParse(firstType.ToString(), out int firstResult);
            if (firstResult < 0)
            {
                sb.AppendLine($"  return {@enum.Name}.number_minus_{-firstResult};");
            }
            else
            {
                sb.AppendLine($"  return {@enum.Name}.number_{firstResult};");
            }
            sb.AppendLine($"}}");

            return sb.ToString();
        }

        private string GenerateEnumOfValue(BaseEnum @enum)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"int {@enum.Name}Value({@enum.Name} value)  {{");
            sb.AppendLine($"  switch (value) {{");

            var firstType = @enum.Types[0];
            foreach (var type in @enum.Types)
            {
                if (type is int || type is long)
                {
                    int.TryParse(type.ToString(), out int result);
                    if (result < 0)
                    {
                        sb.AppendLine($"    case {@enum.Name}.number_{result}:");
                    }
                    else
                    {
                        sb.AppendLine($"    case {@enum.Name}.number_{result}:");
                    }
                    sb.AppendLine($"        return {result};");
                }
            }
            sb.AppendLine("  }");
            int.TryParse(firstType.ToString(), out int firstResult);
            sb.AppendLine($"  return {firstResult};");
            sb.AppendLine($"}}");

            return sb.ToString();
        }

        private string GenerateContentClass(BaseClass @class)
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetAllReferenceTypes(@class));
            sb.AppendLine($"class {@class.GetDartName()} {{");
            foreach (var type in @class.Properties)
            {

                sb.AppendLine($"  {type.Type.GetDartType() } {type.Name.AvoidKeywords()};");
            }
            sb.AppendLine($"");

            sb.AppendLine(GenerateConstructor(@class));

            sb.AppendLine("  @override");
            sb.AppendLine(GenerateToString(@class));
            sb.AppendLine(GenerateFromJson(@class));
            sb.AppendLine(GenerateToJson(@class));
            sb.AppendLine(GenerateListFromJson(@class));
            sb.AppendLine(GenerateMapFromJson(@class));


            sb.AppendLine($"");
            sb.AppendLine($"}}");
            return sb.ToString();
        }

        private string GenerateConstructor(BaseClass @class)
        {
            var sb = new StringBuilder();
            var parameters = string.Join(", ", @class.Properties.Select(type => $"this.{type.Name.AvoidKeywords()}{(type.Type.GetDefaultValueOnlyValued() != null ? " = " + type.Type.GetDefaultValueOnlyValued() : "")}"));
            sb.AppendLine($"  {@class.GetDartName()}({{{parameters}}});");
            return sb.ToString();
        }
        private string GenerateToString(BaseClass @class)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"  String toString() {{");
            sb.AppendLine($"    return '{@class.GetDartName()}[{string.Join(", ", @class.Properties.Select(type => $"{type.Name}=${type.Name.AvoidKeywords()}"))}]';");
            sb.AppendLine("  }");
            return sb.ToString();
        }

        private string GenerateFromJson(BaseClass @class)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"  {@class.GetDartName()}.fromJson(Map<String, dynamic> json) {{");
            sb.AppendLine($"    if (json == null) return;");
            foreach (var property in @class.Properties)
            {
                sb.AppendLine(ParseProperty(property));
            }
            sb.AppendLine($"  }}");
            return sb.ToString();
        }

        private string ParseProperty(BaseProperty property)
        {
            if (property.Type is BaseEnum)
            {
                var @enum = property.Type as BaseEnum;
                return $"    {property.Name.AvoidKeywords()} = {@enum.GetDartName()}ValueOf(json['{property.Name}']);";
            }
            else
            if (property.Type.Type == Enums.ClassTypeEnum.DateTime)
            {
                return $"    {property.Name.AvoidKeywords()} = json['{property.Name}'] != null ? DateTime.parse(json['{property.Name}'] as String) : null;";
            }
            else if (property.Type.Type == Enums.ClassTypeEnum.Array)
            {
                if (property.Type.InnerClass.Type == Enums.ClassTypeEnum.String)
                {
                    return $"    {property.Name.AvoidKeywords()} = json['{property.Name}'] != null ? (json['{property.Name.AvoidKeywords()}'] as List).map((item) => item as String).toList() : [];";
                }
                else if (property.Type.InnerClass.Type == Enums.ClassTypeEnum.Integer)
                {
                    return $"    {property.Name.AvoidKeywords()} = json['{property.Name}'] != null ? (json['{property.Name.AvoidKeywords()}'] as List).map((item) => item as int).toList() : [];";
                }
                else
                {
                    return $"    {property.Name.AvoidKeywords()} = {property.Type.InnerClass.GetDartName()}.listFromJson(json['{property.Name}']);";
                }
            }
            else if (property.Type.Type == Enums.ClassTypeEnum.Object)
            {
                return $"    {property.Name.AvoidKeywords()} = {property.Type.GetDartName()}.fromJson(json['{property.Name}']);";
            }
            else
            {
                return $"    {property.Name.AvoidKeywords()} = json['{property.Name}'];";
            }
        }

        private string GenerateToJson(BaseClass @class)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"  Map<String, dynamic> toJson() {{");
            sb.AppendLine($"    return {{");
            foreach (var property in @class.Properties)
            {
                sb.AppendLine(ParseJsonProperty(property));
            }
            sb.AppendLine($"    }};");
            sb.AppendLine($"  }}");
            return sb.ToString();
        }


        private string ParseJsonProperty(BaseProperty property)
        {
            if (property.Type is BaseEnum)
            {
                var @enum = property.Type as BaseEnum;
                return $"    '{property.Name}' : {@enum.GetDartName()}Value({property.Name.AvoidKeywords()}),";
            }
            if (property.Type.Type == Enums.ClassTypeEnum.DateTime)
            {
                return $"    '{property.Name}' : {property.Name.AvoidKeywords()} != null ? {property.Name.AvoidKeywords()}.toString() : null,";
            }
            else
            {
                return $"    '{property.Name}' : {property.Name.AvoidKeywords()},";
            }
        }

        private string GenerateListFromJson(BaseClass @class)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"  static List<{@class.GetDartName()}> listFromJson(List<dynamic> json) {{");
            sb.AppendLine($"    return json == null");
            sb.AppendLine($"        ? <{@class.GetDartName()}>[]");
            sb.AppendLine($"        : json.map((value) => {@class.GetDartName()}.fromJson(value)).toList();");
            sb.AppendLine($"  }}");
            return sb.ToString();
        }

        private string GenerateMapFromJson(BaseClass @class)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"  static Map<String, {@class.GetDartName()}> mapFromJson(");
            sb.AppendLine($"      Map<String, Map<String, dynamic>> json) {{");
            sb.AppendLine($"    var map = <String, {@class.GetDartName()}>{{}};");
            sb.AppendLine($"    if (json != null && json.isNotEmpty) {{");
            sb.AppendLine($"      json.forEach((String key, Map<String, dynamic> value) =>");
            sb.AppendLine($"          map[key] = {@class.GetDartName()}.fromJson(value));");
            sb.AppendLine($"    }}");
            sb.AppendLine($"    return map;");
            sb.AppendLine($"  }}");
            return sb.ToString();
        }


        private string GetAllReferenceTypes(BaseClass @class)
        {
            var referenceTypes = CollectAllReferenceTypes(@class);
            var sb = new StringBuilder();

            foreach (var referenceType in referenceTypes)
            {
                if (referenceType is BaseClass)
                {
                    sb.AppendLine($"import '{referenceType.GetSnakeName()}.class.dart';");
                }
                if (referenceType is BaseEnum)
                {
                    sb.AppendLine($"import '../enums/{referenceType.GetSnakeName()}.enum.dart';");
                }
            }
            if (referenceTypes.Count > 0)
            {
                sb.AppendLine("");
            }
            return sb.ToString();
        }

        private List<BaseType> CollectAllReferenceTypes(BaseClass @class)
        {
            var referenceTypes = new List<BaseType>();
            foreach (var property in @class.Properties)
            {
                var referenceClass = GetReferenceClass(property.Type);

                if (referenceClass != null)
                {
                    if (referenceClass.ReferenceName == @class.ReferenceName)
                    {
                        continue;
                    }
                    if (!referenceTypes.Any(p => p.Name == referenceClass.Name))
                    {
                        referenceTypes.Add(referenceClass);
                    }
                }
            }
            return referenceTypes;
        }

        public BaseType GetReferenceClass(BaseType @class)
        {
            if (@class.Name != null)
            {
                return @class;
            }
            if (@class.InnerClass != null && @class.InnerClass.Name != null)
            {
                return @class.InnerClass;
            }
            return null;
        }
    }
}
