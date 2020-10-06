using swaggerParser.Output.Base;
using swaggerParser.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output.Base
{
    public class BaseParameter
    {
        public string Name { get; set; }

        public bool InPath { get; set; }

        public bool Required { get; set; }

        public BaseType Type { get; set; }
    }
}
