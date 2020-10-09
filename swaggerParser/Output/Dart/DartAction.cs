using swaggerParser.Output.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output.Dart
{
    public class DartAction : BaseAction
    {

        public DartAction(BaseAction @base)
        {
            Path = @base.Path;
            Method = @base.Method;
            Parameters = @base.Parameters;
            RequestBody = @base.RequestBody;
            Responses = @base.Responses;
        }

        public string DartMethod
        {
            get
            {
                return Method.ToString().ToLower();
            }
        }

        public string DartInputParameters
        {
            get
            {
                if (RequestBody == null)
                {
                    return string.Join(",", Parameters.Select(p => $"{p.Type.GetDartType()} {p.Name.AvoidKeywords()}"));
                }
                var requestParams = Parameters.Select(p => $"{p.Type.GetDartType()} {p.Name.AvoidKeywords()}").ToList();
                requestParams.Add($"{RequestBody.GetDartType()} data");
                return string.Join(",", requestParams);
            }
        }

        public BaseType DartOutputParameterType
        {
            get
            {
                var response200 = Responses.FirstOrDefault(p => p.Code >= 200 && p.Code < 300);
                if (response200 != null)
                {
                    return response200.Type;
                }
                return null;
            }
        }

        public string DartRequestBody
        {
            get
            {
                if (RequestBody != null)
                {
                    return ", data";
                }
                return "";
            }
        }


        public string CollectUri(List<PathChunk> urlChunks)
        {
            var url = "_apiUrl";

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
                        tail.Add($" + '/' + {chunk.Name.AvoidKeywords()}.toString()");
                        var parameterForRemove = otherParameters.FirstOrDefault(p => p.Name == chunk.Name);
                        if (parameterForRemove != null)
                        {
                            otherParameters.Remove(parameterForRemove);
                        }
                    }
                    else
                    {
                        tail.Add($" + '/{chunk.Name}'");
                    }
                }
                url += string.Join("", tail);
                if (otherParameters.Count > 0)
                {
                    url += " + '?' + " + string.Join(" + '&' + ", otherParameters.Select(op => $"'{op.Name}=' + {op.Name.AvoidKeywords()}.toString()").ToList());
                }
            }
            return url;
        }
    }
}
