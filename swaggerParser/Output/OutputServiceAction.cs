using swaggerParser.Output.Enums;
using swaggerParser.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace swaggerParser.Output
{
    public class OutputServiceAction
    {
        private readonly Regex RegexPathParameter = new Regex("{(.*)}");

        public string Path { get; set; }

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

        public MethodTypeEnum Method { get; set; }

        public string AngularMethod
        {
            get
            {
                return Method.ToString().ToLower();
            }
        }




        public List<OutputServiceActionParameter> Parameters { get; set; }

        public BaseOutputClass RequestBody { get; set; }

        public List<OutputResponse> Responses { get; set; }


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

        public static List<OutputServiceActionParameter> ParseParameters(List<DocumentParameter> parameters, List<BaseOutputClass> baseOutputClasses)
        {
            var list = new List<OutputServiceActionParameter>();

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    list.Add(new OutputServiceActionParameter()
                    {
                        Name = parameter.Name,
                        Required = parameter.Required,
                        Class = ClassFactory.GetClassDefinition(parameter.Schema, baseOutputClasses)
                    });
                }
            }
            return list;
        }

        public string AngularInputParameters
        {
            get
            {
                if (RequestBody == null)
                {
                    return string.Join(",", Parameters.Select(p => $"{p.Name}: {p.Class.AngularType}"));
                }
                var requestParams = Parameters.Select(p => $"{p.Name}: {p.Class.AngularType}").ToList();
                requestParams.Add($"body : {RequestBody.AngularType}");
                return string.Join(",", requestParams);
            }
        }

        public string AngularOutputParameter
        {
            get
            {
                var response200 = Responses.FirstOrDefault(p => p.Code >= 200 && p.Code < 300);
                if (response200 != null)
                {
                    return response200.Class.AngularType;
                }
                return "null";
            }
        }

        public string AngularRequestBody
        {
            get
            {
                if (RequestBody != null)
                {
                    return ", body";
                }
                return "";
            }
        }


        public string AngularCollectUri(List<PathChunk> urlChunks)
        {
            var url = "this.apiUrl";

            var actionChunks = PathChunks;

            actionChunks = actionChunks.GetRange(urlChunks.Count, actionChunks.Count - urlChunks.Count);
            if (actionChunks.Count >= 0)
            {
                var tail = new List<string>();
                var otherParameters = Parameters;
                foreach (var chunk in actionChunks)
                {
                    if (chunk.IsParameter)
                    {
                        tail.Add($" + \"/\" + {chunk.Name}");
                        var parameterForRemove = otherParameters.FirstOrDefault(p => p.Name == chunk.Name);
                        if (parameterForRemove != null)
                        {
                            otherParameters.Remove(parameterForRemove);
                        }
                    }
                    else
                    {
                        tail.Add($" + \"/{chunk.Name}\"");
                    }
                }
                url += string.Join("", tail);
                if (otherParameters.Count > 0)
                {
                    url += " + \"?\" + " + string.Join("&", otherParameters.Select(op => $"\"{op.Name}=\" + {op.Name}").ToList());
                }
            }
            return url;
        }

        public static BaseOutputClass ParseRequestBody(DocumentRequestBody requestBody, List<BaseOutputClass> baseOutputClasses)
        {
            if (requestBody == null)
            {
                return null;
            }
            var request = requestBody.Content["application/json"];
            if (request != null && request.Schema != null)
            {
                return ClassFactory.GetClassDefinition(request.Schema, baseOutputClasses);
            }
            return null;
        }

        public static List<OutputResponse> ParseResponses(Dictionary<string, DocumentResponse> responses, List<BaseOutputClass> baseOutputClasses)
        {
            var list = new List<OutputResponse>();

            foreach (var response in responses)
            {
                if (response.Value.Content != null)
                {
                    var schema = response.Value.Content["application/json"].Schema;
                    var item = new OutputResponse()
                    {
                        Code = Int32.Parse(response.Key),
                        Class = ClassFactory.GetClassDefinition(schema, baseOutputClasses)
                    };
                    list.Add(item);
                }
            }

            return list;
        }
    }
}
