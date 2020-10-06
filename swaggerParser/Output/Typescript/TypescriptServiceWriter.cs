using swaggerParser.Output.Base;
using swaggerParser.Output.Enums;
using swaggerParser.Output.Files;
using swaggerParser.Output.Typescript;
using swaggerParser.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output
{
    public class TypescriptServiceWriter : IServiceWriter
    {
       
        public List<ServiceFile> GenerateFiles(List<BaseService> services)
        {
            var list = new List<ServiceFile>();
            foreach (var service in services)
            {
                var typescriptService = new TypescriptService(service);
                list.Add(new ServiceFile()
                {
                    FileName = $"{service.Name.GetKebabName()}.service.ts",
                    Content = GenerateContent(typescriptService)
                });
            }
            return list;
        }

        private string GenerateContent(TypescriptService service)
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

        private string GetMethodName(List<TypescriptAction> list, TypescriptAction current)
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

        private string GetAngularAllReferenceTypes(BaseService service)
        {
            var referenceTypes = CollectAllReferenceTypes(service);
            var sb = new StringBuilder();

            if (referenceTypes.Count > 0)
            {
                sb.AppendLine("");
            }
            foreach (var referenceType in referenceTypes)
            {
                if (referenceType is BaseClass)
                {
                    var reference = new TypescriptClass(referenceType as BaseClass);
                    sb.AppendLine($"import {{ {reference.AngularName} }} from '../classes/{reference.AngularName.GetKebabName()}.class';");
                }
                if (referenceType is BaseEnum)
                {
                    var reference = new TypescriptEnum(referenceType as BaseEnum);
                    sb.AppendLine($"import {{ {reference.AngularName} }} from '../enums/{reference.AngularName.GetKebabName()}.enum';");
                }
            }
            return sb.ToString();
        }

        private List<BaseType> CollectAllReferenceTypes(BaseService service)
        {
            var referenceTypes = new List<BaseType>();
            foreach (var action in service.Actions)
            {
                foreach (var parameter in action.Parameters)
                {
                    var @class = GetReferenceType(parameter.Type);
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
                    var @class = GetReferenceType(action.RequestBody);
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
                    var @class = GetReferenceType(response.Type);
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

        public BaseType GetReferenceType(BaseType input)
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
