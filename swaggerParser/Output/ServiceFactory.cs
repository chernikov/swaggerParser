using swaggerParser.Output.Enums;
using swaggerParser.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output
{
    public class ServiceFactory
    {
        public List<OutputService> GetServices(Document document, List<BaseOutputClass> baseOutputClasses)
        {
            var services = new Dictionary<string, OutputService>();

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
                        var newService = new OutputService()
                        {
                            Name = tag
                        };
                        services.Add(tag, newService);
                    }
                    var service = services[tag];
                    var outputAction = new OutputServiceAction()
                    {
                        Method = OutputServiceAction.ParseMethodType(method),
                        Path = servicePath,
                        RequestBody = OutputServiceAction.ParseRequestBody(actionValue.RequestBody, baseOutputClasses),
                        Responses = OutputServiceAction.ParseResponses(actionValue.Responses, baseOutputClasses),
                        Parameters = OutputServiceAction.ParseParameters(actionValue.Parameters, baseOutputClasses)
                    };
                    service.Actions.Add(outputAction);
                }
            }
            return services.Values.ToList();
        }

        public List<ServiceFile> GenerateFiles(List<OutputService> services)
        {
            var list = new List<ServiceFile>();
            foreach (var service in services)
            {
                list.Add(new ServiceFile()
                {
                    FileName = $"{service.Name.GetKebabName()}.service.ts",
                    Content = GenerateContent(service)
                });
            }
            return list;
        }

        private string GenerateContent(OutputService service)
        {
            var sb = new StringBuilder();

            sb.AppendLine("import { HttpClient, HttpHeaders } from '@angular/common/http';");
            sb.AppendLine("import { Injectable, Inject } from '@angular/core';");
            sb.AppendLine("import { Observable } from 'rxjs';");
            sb.AppendLine("import { APP_BASE_HREF } from '@angular/common';");
            //sb.AppendLine("");
            //sb.AppendLine("import { map } from \"rxjs/operators\";");
            sb.AppendLine("");
            sb.AppendLine(GetAngularAllReferenceTypes(service));
            sb.AppendLine("");
            sb.AppendLine("@Injectable({ providedIn: \"root\" })");
            sb.AppendLine($"export class {service.Name}Service");
            sb.AppendLine("{");
            sb.AppendLine($"\tprivate apiUrl:string = this.baseHref + '/{service.Url}';");
            sb.AppendLine("");
            sb.AppendLine("\tprivate headers = new HttpHeaders({");
            sb.AppendLine("\t\t\"content-type\": \"application/json\",");
            sb.AppendLine("\t\t\"Accept\": \"application/json\"");
            sb.AppendLine("\t});");

            sb.AppendLine("\tprivate options = {");
            sb.AppendLine("\t\theaders : this.headers");
            sb.AppendLine("\t};");
            sb.AppendLine("");
            sb.AppendLine("\tconstructor(private http: HttpClient,");
            sb.AppendLine("\t@Inject(APP_BASE_HREF) private baseHref : string");
            sb.AppendLine("\t) {}");
            sb.AppendLine("");

            foreach (var action in service.Actions)
            {
                var methodName = GetMethodName(service.Actions, action);

                sb.AppendLine($"\t{methodName}({action.AngularInputParameters}) : Observable<{action.AngularOutputParameter}> {{");
                sb.AppendLine($"\t\treturn this.http.{action.AngularMethod}<{action.AngularOutputParameter}>({action.AngularCollectUri(service.UrlChunks)}{action.AngularRequestBody}, this.options).pipe();");
                sb.AppendLine("\t}");
                sb.AppendLine("");
            }
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GetMethodName(List<OutputServiceAction> list, OutputServiceAction current)
        {
            var count = list.Count(p => p.Method == current.Method);
            if (count == 1)
            {
                return current.AngularMethod;
            }
            if (count == 2 && current.Method == MethodTypeEnum.Get)
            {
                var other = list.FirstOrDefault(p => p.Method == current.Method && p.Path != current.Path);

                if (current.PathChunks.Count(p => p.IsParameter) == 0 && other.PathChunks.Count(p => p.IsParameter) != 0)
                {
                    return "getAll";
                }
                if (current.PathChunks.Count(p => p.IsParameter) == 1 && other.PathChunks.Count(p => p.IsParameter) != 1)
                {
                    return "get";
                }
            }
            return current.AngularMethod;
        }

        private string GetAngularAllReferenceTypes(OutputService service)
        {
            var referenceTypes = CollectAllReferenceTypes(service);
            var sb = new StringBuilder();

            if (referenceTypes.Count > 0)
            {
                sb.AppendLine("");
            }
            foreach (var referenceType in referenceTypes)
            {
                if (referenceType is OutputClass)
                {
                    sb.AppendLine($"import {{ {referenceType.AngularName} }} from '../classes/{referenceType.AngularName.GetKebabName()}.class';");
                }
                if (referenceType is OutputEnum)
                {
                    sb.AppendLine($"import {{ {referenceType.AngularName} }} from '../enums/{referenceType.AngularName.GetKebabName()}.enum';");
                }
            }
            return sb.ToString();
        }

        private List<BaseOutputClass> CollectAllReferenceTypes(OutputService service)
        {
            var referenceTypes = new List<BaseOutputClass>();
            foreach (var action in service.Actions)
            {
                foreach (var parameter in action.Parameters)
                {
                    var @class = GetReferenceClass(parameter.Class);
                    if (@class != null)
                    {
                        if (!referenceTypes.Any(p => p.Name == @class.Name))
                        {
                            referenceTypes.Add(@class);
                        }
                    }
                }

                if (action.RequestBody != null)
                {
                    var @class = GetReferenceClass(action.RequestBody);
                    if (@class != null)
                    {
                        if (!referenceTypes.Any(p => p.Name == @class.Name))
                        {
                            referenceTypes.Add(@class);
                        }
                    }
                }

                foreach (var response in action.Responses)
                {
                    var @class = GetReferenceClass(response.Class);
                    if (@class != null)
                    {
                        if (!referenceTypes.Any(p => p.Name == @class.Name))
                        {
                            referenceTypes.Add(@class);
                        }
                    }
                }
            }

            return referenceTypes;
        }

        public BaseOutputClass GetReferenceClass(BaseOutputClass input)
        {
            if (input.Name != null)
            {
                return input;
            }
            if (input.InnerClass != null && input.InnerClass.Name != null)
            {
                return input.InnerClass;
            }
            return null;
        }
    }
}
