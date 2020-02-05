using System.Collections.Generic;

namespace swaggerParser.Swagger
{
    public class DocumentAction
    {
        public List<string> Tags { get; set; }

        public List<DocumentParameter> Parameters { get; set; }

        public DocumentRequestBody RequestBody { get; set; }

        public Dictionary<string, DocumentResponse> Responses { get; set; }
    }
}
