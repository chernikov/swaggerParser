using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Swagger
{
    public class DocumentContent
    {
        [JsonProperty("Schema")]
        public JToken SchemaInner { get; set; }

        /// <summary>
        /// Apply on array
        /// </summary>
        [JsonIgnore]
        public DocumentSchema Schema
        {
            get
            {
                return SchemaInner.ParseJTokenWithRef();
            }
        }
    }
}
