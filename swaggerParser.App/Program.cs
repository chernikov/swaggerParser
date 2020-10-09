using swaggerParser;
using swaggerParser.Generators;
using swaggerParser.Output;
using swaggerParser.Output.Dart;
using swaggerParser.Output.Typescript;
using System;
using System.IO;
using System.Text;

namespace swaggerParserApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string text;
            using var fileStream = new FileStream(@"data/swagger.json", FileMode.Open, FileAccess.Read);
            using var streamReader = new StreamReader(fileStream, Encoding.UTF8);
            text = streamReader.ReadToEnd();

            Console.WriteLine("Start");
            var lang = args.Length > 0 ? args[0] : "typescript";

            ITypeWriter typeWriter = null;
            IServiceWriter serviceWriter = null;
            switch (lang)
            {
                case "typescript":
                    typeWriter = new TypescriptTypeWriter();
                    serviceWriter = new TypescriptServiceWriter();
                    break;
                case "dart":
                    typeWriter = new DartTypeWriter();
                    serviceWriter = new DartServiceWriter();
                    break;
            }
            IGenerator generator = new Generator(text, typeWriter, serviceWriter);
            generator.Parse();
            generator.WriteFiles();
            Console.ReadLine();
        }
    }
}
