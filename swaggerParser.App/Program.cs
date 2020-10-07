using swaggerParser;
using swaggerParser.Generators;
using swaggerParser.Output;
using swaggerParser.Output.Typescript;
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

            ITypeWriter typeWriter = new TypescriptTypeWriter();
            IServiceWriter serviceWriter = new TypescriptServiceWriter();
            IGenerator generator = new Generator(text, typeWriter, serviceWriter);
            generator.Parse();
            generator.WriteFiles();
        }
    }
}
