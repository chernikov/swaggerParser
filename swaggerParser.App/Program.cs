using swaggerParser;
using swaggerParser.Generators;
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

            IGenerator generator = new Generator(text);
            generator.Parse();
            generator.WriteFiles();
        }
    }
}
