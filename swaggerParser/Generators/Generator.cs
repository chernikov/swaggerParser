using Newtonsoft.Json;
using swaggerParser.Output;
using swaggerParser.Output.Base;
using swaggerParser.Output.Files;
using swaggerParser.Output.Typescript;
using swaggerParser.Swagger;
using System.Collections.Generic;
using System.IO;

namespace swaggerParser.Generators
{
    public class Generator : BaseGenerator, IGenerator
    {
        public Generator(string source) : base(source)
        {
        }

        public void Parse()
        {
            Classes = _typeParser.GetTypes(swaggerDoc);
            var serviceParser = new ServiceParser(_typeParser);
            Services = serviceParser.GetServices(swaggerDoc, Classes);

            Files.AddRange(_serviceWriter.GenerateFiles(Services));
            Files.AddRange(_typeWriter.GenerateFiles(Classes));
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
