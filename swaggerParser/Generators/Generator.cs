using Newtonsoft.Json;
using swaggerParser.Output;
using swaggerParser.Output.Base;
using swaggerParser.Output.Files;
using swaggerParser.Output.Parsers;
using swaggerParser.Output.Typescript;
using swaggerParser.Swagger;
using System.Collections.Generic;
using System.IO;

namespace swaggerParser.Generators
{
    public class Generator : IGenerator
    {
        protected readonly Document swaggerDoc;

        protected readonly ITypeParser _typeParser;
        protected readonly IServiceParser _serviceParser;
        protected readonly ITypeWriter typeWriter;
        protected readonly IServiceWriter serviceWriter;

        protected List<BaseType> Classes { get; set; } = new List<BaseType>();

        protected List<BaseService> Services { get; set; } = new List<BaseService>();

        protected List<BaseFile> Files { get; set; } = new List<BaseFile>();


        public Generator(string source, ITypeWriter _typeWriter, IServiceWriter _serviceWriter)
        {
            _typeParser = new TypeParser();
            _serviceParser = new ServiceParser(_typeParser);
            typeWriter = _typeWriter;
            serviceWriter = _serviceWriter;

            swaggerDoc = JsonConvert.DeserializeObject<Document>(source);
     
        }

        public void Parse()
        {
            Classes = _typeParser.GetTypes(swaggerDoc);
            Services = _serviceParser.GetServices(swaggerDoc, Classes);
            Files.AddRange(serviceWriter.GenerateFiles(Services));
            Files.AddRange(typeWriter.GenerateFiles(Classes));
        }

        public void WriteFiles()
        {
            foreach (var file in Files)
            {
                var path = "output";
                if (file is ServiceFile)
                {
                    path += "/services";
                }
                if (file is ClassFile)
                {
                    path += "/classes";
                }
                if (file is EnumFile)
                {
                    path += "/enums";
                }
                var di = new DirectoryInfo(path);
                if (!di.Exists)
                {
                    di.Create();
                }
                var filePath = path + "/" + file.FileName;
                File.WriteAllText(filePath, file.Content);
            }
        }
    }
}
