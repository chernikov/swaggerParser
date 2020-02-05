using swaggerParser.Swagger;

namespace swaggerParser.Output
{
    public abstract class BaseOutputClass
    {
        public abstract string FullName { get; }

        public string Name { get; set; }


        //TODO: Refactor
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

        public bool IsDictionary { get; set; }

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
                    return $"{{ [id: string]: {InnerClass.AngularType}; }}";
                }
                switch (Type)
                {

                    case ClassTypeEnum.Array:
                        return InnerClass.AngularType + "[]";
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
        public string ReferenceName
        {
            get
            {
                return $"#/components/schemas/{Name}";
            }
        }

        public ClassTypeEnum Type { get; set; }

        public BaseOutputClass InnerClass { get; set; }

        public void SetType(DocumentSchema schema)
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
                        InnerClass = ClassFactory.GetClassDefinition(schema.Items);
                    }
                    break;
                case var t when t.type == "object":
                    Type = ClassTypeEnum.Object;
                    if (schema.AdditionalProperties != null)
                    {
                        IsDictionary = true;
                        InnerClass = ClassFactory.GetClassDefinition(schema.AdditionalProperties);
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
