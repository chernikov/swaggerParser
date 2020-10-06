using swaggerParser.Output.Base;
using swaggerParser.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output
{
    public class TypeParser : ITypeParser
    {

        public List<BaseType> GetTypes(Document document)
        {
            var list = new List<BaseType>();
            var components = document.Components;
            foreach (var schema in components.Schemas)
            {
                var entity = CreateDefinition(schema.Key, schema.Value);
                list.Add(entity);
            }
            foreach (var entity in list)
            {
                if (entity is BaseClass)
                {
                    var @class = entity as BaseClass;
                    var schema = components.Schemas[@class.Name];
                    @class.SetProperties(this, list, schema);
                    @class.SetInnerClass(this, list, schema);
                }
            }
            return list;
        }

        public BaseType CreateDefinition(string name, DocumentSchema schema)
        {
            if (schema.Ref != null)
            {
                var refName = schema.Ref.Remove("#/components/schemas/".Length);
                var @class = new BaseClass()
                {
                    Name = refName
                };
                return @class;
            }
            if (schema.Enum != null)
            {
                //create enum
                var @enum = new BaseEnum()
                {
                    Name = name
                };
                @enum.SetType(this, schema);
                @enum.Types = schema.Enum;
                return @enum;
            }
            else
            {
                //create class
                var @class = new BaseClass()
                {
                    Name = name
                };
                @class.SetType(this, schema);
                return @class;
            }
        }


        public BaseType CreateDefinitionWithDeep(DocumentSchema schema, List<BaseType> list)
        {
            if (schema.Enum != null)
            {
                return GetClassDefinition(schema);
            }
            else
            {
                //create class
                var @class = new BaseClass() { };
                @class.SetType(this, schema);
                @class.SetProperties(this, list, schema);
                @class.SetInnerClass(this, list, schema);
                return @class;
            }
        }

        public BaseType GetClassDefinition(DocumentSchema schema, List<BaseType> baseOutputClasses = null)
        {
            if (schema.Ref != null)
            {
                var refName = schema.Ref.Substring("#/components/schemas/".Length);

                if (baseOutputClasses != null)
                {
                    var item = baseOutputClasses.FirstOrDefault(p => p.Name == refName);

                    if (item != null)
                    {
                        if (item is BaseEnum)
                        {
                            var @enum = new BaseEnum()
                            {
                                Name = refName
                            };
                            return @enum;
                        }
                    }
                }
                var @class = new BaseClass()
                {
                    Name = refName
                };
                return @class;

            }
            if (schema.Enum != null)
            {
                //create enum
                var @enum = new BaseEnum() { };
                @enum.SetType(this, schema);
                @enum.Types = schema.Enum;
                return @enum;
            }
            else
            {
                //create class
                var @class = new BaseClass() { };
                @class.SetType(this, schema);
                return @class;
            }
        }
    }
}
