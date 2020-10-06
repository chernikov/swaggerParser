using swaggerParser.Output.Base;
using swaggerParser.Output.Enums;
using swaggerParser.Output.Typescript;
using swaggerParser.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace swaggerParser.Output.Base
{
    public class BaseAction
    {
        private readonly Regex RegexPathParameter = new Regex("{(.*)}");

        public string Path { get; set; }

        public MethodTypeEnum Method { get; set; }
        public List<BaseParameter> Parameters { get; set; }

        public BaseType RequestBody { get; set; }

        public List<BaseResponse> Responses { get; set; }

        public List<PathChunk> PathChunks
        {
            get
            {
                var list = new List<PathChunk>();

                var pathParts = Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var pathPart in pathParts)
                {
                    var match = RegexPathParameter.Match(pathPart);
                    if (match.Success)
                    {
                        var parameterName = match.Groups[1].Value;
                        list.Add(new PathChunk()
                        {
                            Name = parameterName,
                            IsParameter = true
                        });
                    }
                    else
                    {
                        list.Add(new PathChunk()
                        {
                            Name = pathPart,
                            IsParameter = false
                        });
                    }
                }

                return list;
            }
        }


        public static MethodTypeEnum ParseMethodType(string method)
        {
            switch (method.ToLower())
            {
                case "get":
                    return MethodTypeEnum.Get;
                case "post":
                    return MethodTypeEnum.Post;
                case "put":
                    return MethodTypeEnum.Put;
                case "delete":
                    return MethodTypeEnum.Delete;
                default:
                    return default;
            }
        }
    }
}
