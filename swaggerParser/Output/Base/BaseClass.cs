using swaggerParser.Output.Enums;
using swaggerParser.Output.Parsers;
using swaggerParser.Swagger;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace swaggerParser.Output.Base
{
    [DebuggerDisplay("Class: {FullName}")]
    public class BaseClass : BaseType
    {
        public List<BaseProperty> Properties { get; set; }

        public void SetProperties(ITypeParser parser, List<BaseType> list, DocumentSchema schema)
        {
            if (schema.Properties != null && schema.Properties.Count > 0)
            {
                Properties = new List<BaseProperty>();
                foreach (var property in schema.Properties)
                {
                    var propertyValue = property.Value;
                    var @ref = propertyValue.Ref ?? propertyValue.AllOf?.Ref;

                    var innerEntity = @ref != null ?
                        list.First(p => p.ReferenceName == @ref) : parser.CreateDefinitionWithDeep(property.Value, list);


                    Properties.Add(new BaseProperty()
                    {
                        IsNullable = !schema.Required.Contains(property.Key),
                        Name = property.Key,
                        Type = innerEntity
                    });
                }
            }
        }

        internal void SetInnerClass(ITypeParser parser, List<BaseType> list, DocumentSchema schema)
        {
            if (IsDictionary)
            {
                var @ref = schema.AdditionalProperties.Ref;
                var innerEntity = @ref != null ?
                       list.First(p => p.ReferenceName == @ref) : parser.CreateDefinitionWithDeep(schema.AdditionalProperties, list);
                InnerClass = innerEntity;
                return;
            }
            if (Type == ClassTypeEnum.Array)
            {
                string @ref = null;
                if (schema.Items != null)
                {
                    @ref = schema.Items.Ref;
                }
                else if (schema.AllOf != null)
                {
                    @ref = schema.AllOf.Ref;
                }

                var innerEntity = @ref != null ?
                       list.First(p => p.ReferenceName == @ref) : parser.CreateDefinitionWithDeep(schema.Items, list);
                InnerClass = innerEntity;
            }
        }

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
