namespace swaggerParser.Output
{
    public class BaseFile
    {
        public string FileName { get; set; }

        public string Content { get; set; }

        public override string ToString()
        {
            return FileName;
        }
    }
}
