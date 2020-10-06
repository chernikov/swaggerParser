using swaggerParser.Output.Base;
using swaggerParser.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output
{
    public class ServiceParser : IServiceParser
    {
        private readonly ITypeParser typeParser;

        public ServiceParser(ITypeParser typeParser)
        {
            this.typeParser = typeParser;
        }
        public List<BaseService> GetServices(Document document, List<BaseType> baseOutputClasses)
        {
            var services = new Dictionary<string, BaseService>();

            foreach (var path in document.Paths)
            {
                var servicePath = path.Key;
                foreach (var action in path.Value)
                {
                    var method = action.Key;
                    var actionValue = action.Value;
                    var tag = actionValue.Tags[0];
                    if (!services.ContainsKey(tag))
                    {
                        var newService = new BaseService()
                        {
                            Name = tag
                        };
                        services.Add(tag, newService);
                    }
                    var service = services[tag];
                    var outputAction = new BaseAction()
                    {
                        Method = BaseAction.ParseMethodType(method),
                        Path = servicePath,
                        RequestBody = ParseRequestBody( actionValue.RequestBody, baseOutputClasses),
                        Responses = ParseResponses(actionValue.Responses, baseOutputClasses),
                        Parameters = ParseParameters( actionValue.Parameters, baseOutputClasses)
                    };
                    service.Actions.Add(outputAction);
                }
            }
            return services.Values.ToList();
        }

        public BaseType ParseRequestBody(DocumentRequestBody requestBody, List<BaseType> baseOutputClasses)
        {
            if (requestBody == null)
            {
                return null;
            }
            var request = requestBody.Content["application/json"];
            if (request != null && request.Schema != null)
            {
                return typeParser.GetClassDefinition(request.Schema, baseOutputClasses);
            }
            return null;
        }

        public List<BaseResponse> ParseResponses(Dictionary<string, DocumentResponse> responses, List<BaseType> baseOutputClasses)
        {
            var list = new List<BaseResponse>();

            foreach (var response in responses)
            {
                if (response.Value.Content != null)
                {
                    var schema = response.Value.Content["application/json"].Schema;
                    var item = new BaseResponse()
                    {
                        Code = Int32.Parse(response.Key),
                        Type = typeParser.GetClassDefinition(schema, baseOutputClasses)
                    };
                    list.Add(item);
                }
            }

            return list;
        }

        public List<BaseParameter> ParseParameters(List<DocumentParameter> parameters, List<BaseType> baseOutputClasses)
        {
            var list = new List<BaseParameter>();

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    list.Add(new BaseParameter()
                    {
                        Name = parameter.Name,
                        Required = parameter.Required,
                        Type = typeParser.GetClassDefinition(parameter.Schema, baseOutputClasses)
                    });
                }
            }
            return list;
        }
    }
}
