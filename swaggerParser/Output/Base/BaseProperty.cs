using System.Diagnostics;

namespace swaggerParser.Output.Base
{
    [DebuggerDisplay("Property: {Name} ({Class.FullName})")]
    public class BaseProperty
    {
        public bool IsNullable { get; set; }

        public string Name { get; set; }

        public BaseType Type { get; set; }
    }
}
