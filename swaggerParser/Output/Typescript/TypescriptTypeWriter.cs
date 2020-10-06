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
                if (_type is BaseEnum)
                {
                    var typescriptEnum = new TypescriptEnum(_type as BaseEnum);
                    list.Add(new EnumFile()
                    {
                        FileName = $"{typescriptEnum.Name.GetKebabName()}.enum.ts",
                        Content = GenerateContentEnum(typescriptEnum)
                    });
                }
                if (_type is BaseClass)
                {
                    var @class = new TypescriptClass(_type as BaseClass); 
                    list.Add(new ClassFile()
                    {
                        FileName = $"{@class.AngularName.GetKebabName()}.class.ts",
                        Content = GenerateContentClass(@class)
                    });
                }
            }
            return list;
        }

      

        private string GenerateContentEnum(TypescriptEnum @enum)
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

        private string GenerateContentClass(TypescriptClass @class)
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetAllReferenceTypes(@class));
            sb.AppendLine($"export class {@class.AngularName} {{");
            foreach (var type in @class.Properties)
            {
                var innerClass = new TypescriptClass(new BaseClass(type.Type));
                sb.AppendLine($"\t{type.Name} : {innerClass.AngularType};");
            }
            sb.AppendLine($"");

            sb.AppendLine(GenerateConstructor(@class));

            sb.AppendLine($"");
            sb.AppendLine($"}}");
            return sb.ToString();
        }

        private string GenerateConstructor(TypescriptClass @class)
        {
            var sb = new StringBuilder();
            sb.AppendLine("\tconstructor() {");
            foreach (var type in @class.Properties)
            {
                var innerClass = new TypescriptClass(new BaseClass(type.Type));
                sb.AppendLine($"\t\tthis.{type.Name} = {innerClass.DefaultValue};");
            }
            sb.AppendLine("\t}");
            return sb.ToString();
        }



        private string GetAllReferenceTypes(TypescriptClass @class)
        {
            var referenceTypes = CollectAllReferenceTypes(@class);
            var sb = new StringBuilder();

            foreach (var referenceType in referenceTypes)
            {
                if (referenceType is BaseClass)
                {
                    var reference = new TypescriptClass(referenceType as BaseClass);
                    sb.AppendLine($"import {{ {reference.AngularName} }} from './{reference.AngularName.GetKebabName()}.class';");
                }
                if (referenceType is BaseEnum)
                {
                    var reference = new TypescriptEnum(referenceType as BaseEnum);
                    sb.AppendLine($"import {{ {reference.AngularName} }} from '../enums/{reference.AngularName.GetKebabName()}.enum';");
                }
            }
            if (referenceTypes.Count > 0)
            {
                sb.AppendLine("");
            }
            return sb.ToString();
        }

        private List<BaseType> CollectAllReferenceTypes(TypescriptClass @class)
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
