using System.Collections.Generic;

namespace swaggerParser.Swagger
{
    public class DocumentResponse
    {
        public string Description { get; set; }

        public Dictionary<string, DocumentContent> Content { get; set; }
    }
}
