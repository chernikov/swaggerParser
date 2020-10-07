using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output.Base
{
    public class BaseService
    {
        public string Name { get; set; }

        public IList<BaseAction> Actions { get; set; }

        public List<PathChunk> UrlChunks
        {
            get
            {
                var commonPart = new List<PathChunk>();
                if (Actions.Any())
                {
                    var action = Actions.First();
                    commonPart = action.PathChunks.Where(p => !p.IsParameter).ToList();
                }
                foreach (var action in Actions)
                {
                    var candidate = action.PathChunks.Where(p => !p.IsParameter).ToList();

                    var i = 0;
                    foreach (var chunk in commonPart)
                    {
                        if (candidate.Count <= i)
                        {
                            commonPart = candidate;
                            break;
                        }
                        if (chunk.Name != candidate[i].Name)
                        {
                            commonPart = candidate.GetRange(0, i);
                        }
                        i++;
                    }
                }
                return commonPart;
            }
        }
        public string Url
        {
            get
            {
                return string.Join("/", UrlChunks.Select(p => p.Name));
            }
        }

      
        public void AddAction (BaseAction action)
        {
            (Actions as List<BaseAction>).Add(action);
        }


        public BaseService()
        {
            Actions = new List<BaseAction>();
        }
    }
}
