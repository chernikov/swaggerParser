using swaggerParser.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace swaggerParser.Output
{
    public class ClassFactory
    {
        public List<BaseOutputClass> GetClasses(Document document)
        {
            var list = new List<BaseOutputClass>();
            var components = document.Components;
            foreach (var schema in components.Schemas)
            {
                var entity = CreateDefinition(schema.Key, schema.Value);
                list.Add(entity);
            }
            foreach (var entity in list)
            {
                if (entity is OutputClass)
                {
                    var @class = entity as OutputClass;
                    var schema = components.Schemas[@class.Name];
                    @class.SetProperties(this, list, schema);

                    @class.SetInnerClass(this, list, schema);
                }
            }

            return list;
        }

        private BaseOutputClass CreateDefinition(string name, DocumentSchema schema)
        {
            if (schema.Ref != null)
            {
                var refName = schema.Ref.Remove("#/components/schemas/".Length);
                var @class = new OutputClass()
                {
                    Name = refName
                };
                return @class;
            }
            if (schema.Enum != null)
            {
                //create enum
                var @enum = new OutputEnum()
                {
                    Name = name
                };
                @enum.SetType(schema);
                @enum.Types = schema.Enum;
                return @enum;
            }
            else
            {
                //create class
                var @class = new OutputClass()
                {
                    Name = name
                };
                @class.SetType(schema);
                return @class;
            }
        }


        public static BaseOutputClass GetClassDefinition(DocumentSchema schema, List<BaseOutputClass> baseOutputClasses = null)
        {
            if (schema.Ref != null)
            {
                var refName = schema.Ref.Substring("#/components/schemas/".Length);

                if (baseOutputClasses != null)
                {
                    var item = baseOutputClasses.FirstOrDefault(p => p.Name == refName);

                    if (item != null)
                    {
                        if (item is OutputEnum)
                        {
                            var @enum = new OutputEnum()
                            {
                                Name = refName
                            };
                            return @enum;
                        }
                    }
                }
                var @class = new OutputClass()
                {
                    Name = refName
                };
                return @class;

            }
            if (schema.Enum != null)
            {
                //create enum
                var @enum = new OutputEnum() { };
                @enum.SetType(schema);
                @enum.Types = schema.Enum;
                return @enum;
            }
            else
            {
                //create class
                var @class = new OutputClass() { };
                @class.SetType(schema);
                return @class;
            }
        }

        internal BaseOutputClass CreateDefinitionWithDeep(DocumentSchema schema, List<BaseOutputClass> list)
        {
            if (schema.Enum != null)
            {
                return ClassFactory.GetClassDefinition(schema);
            }
            else
            {
                //create class
                var @class = new OutputClass() { };
                @class.SetType(schema);
                @class.SetProperties(this, list, schema);
                @class.SetInnerClass(this, list, schema);
                return @class;
            }
        }

        public List<BaseFile> GenerateFiles(List<BaseOutputClass> classes)
        {
            var list = new List<BaseFile>();
            foreach (var baseClass in classes)
            {
                if (baseClass is OutputEnum)
                {
                    var @enum = baseClass as OutputEnum;
                    list.Add(new EnumFile()
                    {
                        FileName = $"{@enum.Name.GetKebabName()}.enum.ts",
                        Content = GenerateContentEnum(@enum)
                    });
                }
                if (baseClass is OutputClass)
                {
                    var @class = baseClass as OutputClass;
                    list.Add(new ClassFile()
                    {
                        FileName = $"{@class.AngularName.GetKebabName()}.class.ts",
                        Content = GenerateContentClass(@class)
                    });
                }
            }
            return list;
        }

        private string GenerateContentEnum(OutputEnum @enum)
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
                    sb.AppendLine($"\t{type.ToString()}, ");
                }
            }
            sb.AppendLine($"}}");
            return sb.ToString();
        }

        private string GenerateContentClass(OutputClass @class)
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetAngularAllReferenceTypes(@class));
            sb.AppendLine($"export class {@class.AngularName} {{");
            foreach (var type in @class.Properties)
            {
                sb.AppendLine($"\t{type.Name} : {type.Class.AngularType};");
            }
            sb.AppendLine($"");

            sb.AppendLine(GenerateConstructor(@class));

            sb.AppendLine($"");
            sb.AppendLine($"}}");
            return sb.ToString();
        }

        private string GenerateConstructor(OutputClass @class)
        {
            var sb = new StringBuilder();
            sb.AppendLine("\tconstructor() {");
            foreach (var type in @class.Properties)
            {
                sb.AppendLine($"\t\tthis.{type.Name} = {type.Class.DefaultValue};");
            }
            sb.AppendLine("\t}");
            return sb.ToString();
        }

        private string GetAngularAllReferenceTypes(OutputClass @class)
        {
            var referenceTypes = CollectAllReferenceTypes(@class);
            var sb = new StringBuilder();

            foreach (var referenceType in referenceTypes)
            {
                if (referenceType is OutputClass)
                {
                    sb.AppendLine($"import {{ {referenceType.AngularName} }} from './{referenceType.AngularName.GetKebabName()}.class';");
                }
                if (referenceType is OutputEnum)
                {
                    sb.AppendLine($"import {{ {referenceType.AngularName} }} from '../enums/{referenceType.AngularName.GetKebabName()}.enum';");
                }
            }
            if (referenceTypes.Count > 0)
            {
                sb.AppendLine("");
            }
            return sb.ToString();
        }

        private List<BaseOutputClass> CollectAllReferenceTypes(OutputClass @class)
        {
            var referenceTypes = new List<BaseOutputClass>();
            foreach (var property in @class.Properties)
            {
                var referenceClass = GetReferenceClass(property.Class);

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

        public BaseOutputClass GetReferenceClass(BaseOutputClass input)
        {
            if (input.Name != null)
            {
                return input;
            }
            if (input.InnerClass != null && input.InnerClass.Name != null)
            {
                return input.InnerClass;
            }
            return null;
        }
    }
}
