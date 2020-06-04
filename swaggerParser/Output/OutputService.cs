using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Output
{
    public class OutputService
    {
        public string Name { get; set; }

        public List<PathChunk> UrlChunks
        {
            get
            {
                var commonPart = new List<PathChunk>();
                if (Actions.Count > 0)
                {
                    var action = Actions[0];
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

        public List<OutputServiceAction> Actions { get; set; }

        public OutputService()
        {
            Actions = new List<OutputServiceAction>();
        }
    }
}
