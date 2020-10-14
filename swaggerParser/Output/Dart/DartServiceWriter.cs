using swaggerParser.Output.Base;
using swaggerParser.Output.Enums;
using swaggerParser.Output.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output.Dart
{
    public class DartServiceWriter : IServiceWriter
    {
        public List<ServiceFile> GenerateFiles(List<BaseService> services)
        {
            var list = new List<ServiceFile>();
            foreach (var service in services)
            {
                var dartService = new DartService(service);
                var file = new ServiceFile()
                {
                    FileName = $"{service.GetSnakeName()}.service.dart",
                    Content = GenerateContent(dartService)
                };

                list.Add(file);
                Console.WriteLine($"Generate service: {file.FileName}");

            }
            return list;
        }
        private string GenerateContent(DartService service)
        {
            var sb = new StringBuilder();

            sb.AppendLine("import '../../shared/services/base_service.dart';");
            sb.AppendLine("import '../../shared/types/api_result.dart';");

            sb.AppendLine(GetAllReferenceTypes(service));
            sb.AppendLine($"class {service.Name}Service");
            sb.AppendLine("{");
            sb.AppendLine($"  final String _apiUrl = '/{service.Url}';");
            sb.AppendLine("");
            sb.AppendLine($"  BaseService baseService;");
            sb.AppendLine("");
            sb.AppendLine($"  {service.Name}Service() {{");
            sb.AppendLine($"  baseService = BaseService();");
            sb.AppendLine($"  }}");
            sb.AppendLine("");
            foreach (var action in service.Actions)
            {
                var methodName = GetMethodName(service.Actions, action);
                var dartAction = action as DartAction;
                if (dartAction.DartOutputParameterType == null)
                {
                    sb.AppendLine($"  Future<dynamic> {methodName}({dartAction.DartInputParameters}) async {{");
                    sb.AppendLine($"    return baseService.{dartAction.DartMethod}({dartAction.CollectUri(service.UrlChunks)}{dartAction.DartRequestBody}).then((res) => res);");
                } else
                {
                    var parse = GetParseJsonByType(dartAction.DartOutputParameterType);
                    var parameterType = dartAction.DartOutputParameterType.GetDartType();
                    sb.AppendLine($"  Future<ApiResult<{parameterType}>> {methodName}({dartAction.DartInputParameters}) async {{");
                    sb.AppendLine($"    return baseService.{dartAction.DartMethod}({dartAction.CollectUri(service.UrlChunks)}{dartAction.DartRequestBody}).then((res) => ApiResult(res.errors, {parse}));");

                }
                sb.AppendLine("  }");
                sb.AppendLine("");
            }
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GetParseJsonByType(BaseType outputType)
        {
            if (outputType.Type == ClassTypeEnum.DateTime)
            {
                return $"DateTime.parse(res.data as String);";
            }
            else if (outputType.Type == ClassTypeEnum.Array)
            {
                if (outputType.InnerClass != null)
                {
                    return $"{outputType.InnerClass.GetDartName()}.listFromJson(res.data)";
                }
                else
                {
                    return $"{outputType.GetDartName()}.fromJson(res.data)";
                }
                //if (outputType.InnerClass.Type == ClassTypeEnum.String)
                //{
                //    return $"(res.data as List).map((item) => item as String).toList();";
                //}
                //else if (outputType.InnerClass.Type == ClassTypeEnum.Integer)
                //{
                //    return $"(res.data as List).map((item) => item as int).toList();";
                //}
                //else
                //{
                //    
                //}
            }
            else if (outputType.Type == ClassTypeEnum.Object)
            {
                return $"{outputType.GetDartName()}.fromJson(res.data);";
            }
            else
            {
                return $"res.data";
            }
        }

        private string GetMethodName(IEnumerable<BaseAction> list, BaseAction current)
        {
            var currentDartAction = current as DartAction;
            var count = list.Count(p => p.Method == current.Method);
            if (count == 1)
            {
                return currentDartAction.DartMethod;
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
            return currentDartAction.DartMethod;
        }

        private string GetAllReferenceTypes(BaseService service)
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
                    sb.AppendLine($"import '../classes/{referenceType.GetSnakeName()}.class.dart';");
                }
                if (referenceType is BaseEnum)
                {
                    sb.AppendLine($"import '../enums/{referenceType.GetSnakeName()}.enum.dart';");
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
