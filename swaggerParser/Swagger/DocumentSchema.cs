using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace swaggerParser.Swagger
{
    public class DocumentSchema
    {
        /// <summary>
        /// List Of Required properties
        /// </summary>
        public List<string> Required { get; set; } = new List<string>();

        /// <summary>
        /// List of enum's types  (int, string)
        /// </summary>
        public List<object> Enum { get; set; }

        /// <summary>
        /// Can be: object, integer, number, string
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Can be pairs type: 
        ///     object, [none]
        ///     integer : int32, int64  
        ///     string : [none], date-time, byte, 
        ///     number : double, float 
        /// </summary>
        public string Format { get; set; }

        public bool Nullable { get; set; }

        [JsonProperty("ref")]
        public string Ref { get; set; }


        [JsonProperty("Properties")]
        public Dictionary<string, JToken> SchemaProperties { get; set; }



        [JsonProperty("allOf")]
        public List<JToken> schemaAllOf { get; set; }


        [JsonProperty("Items")]
        public JToken SchemaItems { get; set; }


        /// <summary>
        /// Apply on type = object
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, DocumentSchema> Properties
        {
            get
            {
                return ParseArrayWithRef(SchemaProperties);

            }
        }



        /// <summary>
        /// Apply on array
        /// </summary>
        [JsonIgnore]
        public DocumentSchema Items
        {
            get
            {
                if (SchemaItems == null)
                {
                    return null;
                }
                return SchemaItems.ParseJTokenWithRef();
            }
        }

        /// <summary>
        /// Apply on 
        /// </summary>
        [JsonIgnore]
        public DocumentSchema AllOf
        {
            get
            {
                if (schemaAllOf == null || schemaAllOf.Count == 0)
                {
                    return null;
                }
                return schemaAllOf[0].ParseJTokenWithRef();
            }
        }



        private Dictionary<string, DocumentSchema> ParseArrayWithRef(Dictionary<string, JToken> source)
        {
            if (source == null || source.Count == 0)
            {
                return null;
            }
            var dict = new Dictionary<string, DocumentSchema>();

            foreach (var kv in source)
            {
                dict.Add(kv.Key, kv.Value.ParseJTokenWithRef());
            }
            return dict;
        }

        /// <summary>
        /// Apply on Dictionary, SortedList etc (first generic type is lost)
        /// </summary>
        [JsonProperty("AdditionalProperties")]
        public JToken SchemaAdditionalProperties { get; set; }

        [JsonIgnore]
        public DocumentSchema AdditionalProperties
        {
            get
            {
                if (SchemaAdditionalProperties is JValue)
                {
                    return null;
                }
                return SchemaAdditionalProperties.ParseJTokenWithRef();
            }
        }
    }
}
