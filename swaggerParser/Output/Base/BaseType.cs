using swaggerParser.Output.Enums;
using swaggerParser.Output.Parsers;
using swaggerParser.Output.Typescript;
using swaggerParser.Swagger;

namespace swaggerParser.Output.Base
{
    public abstract class BaseType
    {
        public string Name { get; set; }

        public ClassTypeEnum Type { get; set; }

        public bool IsDictionary { get; set; }

        public BaseType InnerClass { get; set; }

        public abstract string FullName { get; }

        public string ReferenceName
        {
            get
            {
                return $"#/components/schemas/{Name}";
            }
        }

     

        public void SetType(ITypeParser parser, DocumentSchema schema)
        {
            //string type, string format
            var type = schema.Type;
            var format = schema.Format;
            switch (type, format)
            {
                case var t when t.type == "boolean":
                    Type = ClassTypeEnum.Boolean;
                    break;

                case var t when t.type == "array":
                    Type = ClassTypeEnum.Array;
                    if (schema.Items != null)
                    {
                        InnerClass = parser.GetClassDefinition(schema.Items);
                    }
                    break;
                case var t when t.type == "object":
                    Type = ClassTypeEnum.Object;
                    if (schema.AdditionalProperties != null)
                    {
                        IsDictionary = true;
                        InnerClass = parser.GetClassDefinition(schema.AdditionalProperties);
                    }
                    break;
                case var t when t.type == "integer" && t.format == "int32":
                    Type = ClassTypeEnum.Integer;
                    break;
                case var t when t.type == "integer" && t.format == "int64":
                    Type = ClassTypeEnum.Long;
                    break;
                case var t when t.type == "string" && t.format == "date-time":
                    Type = ClassTypeEnum.DateTime;
                    break;
                case var t when t.type == "string" && t.format == "byte":
                    Type = ClassTypeEnum.Byte;
                    break;
                case var t when t.type == "string":
                    Type = ClassTypeEnum.String;
                    break;
                case var t when t.type == "number" && t.format == "double":
                    Type = ClassTypeEnum.Double;
                    break;
                case var t when t.type == "number" && t.format == "float":
                    Type = ClassTypeEnum.Float;
                    break;
            }

        }
    }
}
