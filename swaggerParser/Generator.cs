using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using swaggerParser.Output;
using swaggerParser.Swagger;

namespace swaggerParser
{
    public class Generator
    {
        private readonly Document swaggerDoc;

        private List<BaseFile> Files { get; set; } = new List<BaseFile>();

        private readonly ClassFactory _classFactory = new ClassFactory();

        private readonly ServiceFactory _serviceFactory = new ServiceFactory();

        private List<BaseOutputClass> Classes { get; set; } = new List<BaseOutputClass>();

        private List<OutputService> Services { get; set; } = new List<OutputService>();



        public Generator(string source)
        {
            swaggerDoc = JsonConvert.DeserializeObject<Document>(source);
        }

        public void Parse()
        {
            Classes = _classFactory.GetClasses(swaggerDoc);
            Services = _serviceFactory.GetServices(swaggerDoc, Classes);

            Files.AddRange(_serviceFactory.GenerateFiles(Services));
            Files.AddRange(_classFactory.GenerateFiles(Classes));
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

                //ZipArchiveEntry zipEntry = null;
                //if (file is ClassFile)
                //{
                //    if (((ClassFile)file).IsEnum)
                //    {
                //        zipEntry = zip.CreateEntry("enums/" + file.FileName);
                //    }
                //    else
                //    {
                //        zipEntry = zip.CreateEntry("classes/" + file.FileName);
                //    }
                //}
                //if (file is ServiceFile)
                //{
                //    zipEntry = zip.CreateEntry("services/" + file.FileName);
                //}
                //using (var writer = new StreamWriter(zipEntry.Open()))
                //{
                //    using (var stream = GenerateStreamFromString(file.Content))
                //    {
                //        stream.WriteTo(writer.BaseStream);
                //    }
                //}
            }
        }


    }
}
