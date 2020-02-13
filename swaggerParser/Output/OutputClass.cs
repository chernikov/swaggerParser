using swaggerParser.Swagger;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace swaggerParser.Output
{
    [DebuggerDisplay("Class: {FullName}")]
    public class OutputClass : BaseOutputClass
    {

        public List<ClassProperty> Properties { get; set; }

        public void SetProperties(ClassFactory factory, List<BaseOutputClass> list, DocumentSchema schema)
        {
            if (schema.Properties != null && schema.Properties.Count > 0)
            {
                Properties = new List<ClassProperty>();
                foreach (var property in schema.Properties)
                {
                    var propertyValue = property.Value;
                    var @ref = propertyValue.Ref ?? propertyValue.AllOf?.Ref;

                    var innerEntity = @ref != null ?
                        list.First(p => p.ReferenceName == @ref) : factory.CreateDefinitionWithDeep(property.Value, list);


                    Properties.Add(new ClassProperty()
                    {
                        IsNullable = !schema.Required.Contains(property.Key),
                        Name = property.Key,
                        Class = innerEntity
                    });
                }
            }
        }

        internal void SetInnerClass(ClassFactory factory, List<BaseOutputClass> list, DocumentSchema schema)
        {

            if (IsDictionary)
            {
                var @ref = schema.AdditionalProperties.Ref;
                var innerEntity = @ref != null ?
                       list.First(p => p.ReferenceName == @ref) : factory.CreateDefinitionWithDeep(schema.AdditionalProperties, list);
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
                       list.First(p => p.ReferenceName == @ref) : factory.CreateDefinitionWithDeep(schema.Items, list);
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
    }
}
