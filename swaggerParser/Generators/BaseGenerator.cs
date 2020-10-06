using Newtonsoft.Json;
using swaggerParser.Output;
using swaggerParser.Output.Base;
using swaggerParser.Output.Files;
using swaggerParser.Output.Typescript;
using swaggerParser.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Generators
{
    public class BaseGenerator 
    {
        protected readonly Document swaggerDoc;

        protected readonly ITypeParser _typeParser = new TypeParser();

        protected readonly IServiceParser _serviceParser;

        protected readonly ITypeWriter _typeWriter = new TypescriptTypeWriter();

        protected readonly IServiceWriter _serviceWriter = new TypescriptServiceWriter();

        protected List<BaseType> Classes { get; set; } = new List<BaseType>();

        protected List<BaseService> Services { get; set; } = new List<BaseService>();

        protected List<BaseFile> Files { get; set; } = new List<BaseFile>();


        public BaseGenerator(string source)
        {
            swaggerDoc = JsonConvert.DeserializeObject<Document>(source);
        }
    }
}
