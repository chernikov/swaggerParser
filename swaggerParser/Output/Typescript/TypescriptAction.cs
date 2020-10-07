﻿using swaggerParser.Output.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output.Typescript
{
    public class TypescriptAction : BaseAction
    {

        public TypescriptAction(BaseAction @base)
        {
            Path = @base.Path;
            Method = @base.Method;
            Parameters = @base.Parameters;
            RequestBody = @base.RequestBody;
            Responses = @base.Responses;
        }

        public string TypescriptMethod
        {
            get
            {
                return Method.ToString().ToLower();
            }
        }

        public string TypescriptInputParameters
        {
            get
            {
                if (RequestBody == null)
                {
                    return string.Join(",", Parameters.Select(p => $"{p.Name}: {p.Type.GetTypescriptType()}"));
                }
                var requestParams = Parameters.Select(p => $"{p.Name}: {p.Type.GetTypescriptType()}").ToList();
                requestParams.Add($"body : {RequestBody.GetTypescriptType()}");
                return string.Join(",", requestParams);
            }
        }

        public string TypescriptOutputParameter
        {
            get
            {
                var response200 = Responses.FirstOrDefault(p => p.Code >= 200 && p.Code < 300);
                if (response200 != null)
                {
                    return response200.Type.GetTypescriptType();
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
                    url += " + \"?\" + " + string.Join(" + \"&\" + ", otherParameters.Select(op => $"\"{op.Name}=\" + {op.Name}").ToList());
                }
            }
            return url;
        }


    }
}
