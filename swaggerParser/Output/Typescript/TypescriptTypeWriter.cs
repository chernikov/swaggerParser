using swaggerParser.Output.Base;
using swaggerParser.Output.Files;
using swaggerParser.Output.Typescript;
using swaggerParser.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace swaggerParser.Output.Typescript
{
    public class TypescriptTypeWriter : ITypeWriter
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
                        FileName = $"{_type.GetKebabName()}.enum.ts",
                        Content = GenerateContentEnum(_type as BaseEnum)
                    };
                }
                if (_type is BaseClass)
                {
                    file = new ClassFile()
                    {
                        FileName = $"{_type.GetKebabName()}.class.ts",
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

            sb.AppendLine($"export enum {@enum.Name} {{");
            foreach (var type in @enum.Types)
            {
                if (type is int || type is long)
                {
                    int.TryParse(type.ToString(), out int result);
                    if (result < 0)
                    {
                        sb.AppendLine($"\tNUMBER_MINUS_{-result} = {result}, ");
                    }
                    else
                    {
                        sb.AppendLine($"\tNUMBER_{result} = {result}, ");
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

        private string GenerateContentClass(BaseClass @class)
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetAllReferenceTypes(@class));
            sb.AppendLine($"export class {@class.GetTypescriptName()} {{");
            foreach (var type in @class.Properties)
            {
                
                sb.AppendLine($"\t{type.Name} : {type.Type.GetTypescriptType() };");
            }
            sb.AppendLine($"");

            sb.AppendLine(GenerateConstructor(@class));

            sb.AppendLine($"");
            sb.AppendLine($"}}");
            return sb.ToString();
        }

        private string GenerateConstructor(BaseClass @class)
        {
            var sb = new StringBuilder();
            sb.AppendLine("\tconstructor() {");
            foreach (var type in @class.Properties)
            {
                
                sb.AppendLine($"\t\tthis.{type.Name} = {type.Type.GetDefaultValue()};");
            }
            sb.AppendLine("\t}");
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
                    sb.AppendLine($"import {{ {referenceType.GetTypescriptName()} }} from './{referenceType.GetKebabName()}.class';");
                }
                if (referenceType is BaseEnum)
                {
                    sb.AppendLine($"import {{ {referenceType.GetTypescriptName()} }} from '../enums/{referenceType.GetKebabName()}.enum';");
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
