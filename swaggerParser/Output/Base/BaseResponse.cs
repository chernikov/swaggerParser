using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output.Base
{
    public class BaseResponse
    {
        public int Code { get; set; }

        public BaseType Type { get; set; }

    }
}
