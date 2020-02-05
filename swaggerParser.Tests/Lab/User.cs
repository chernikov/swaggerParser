using Newtonsoft.Json;

namespace swaggerParser.Tests.Lab
{
    [JsonConverter(typeof(UserConverter))]
    public class User
    {
        public string UserName { get; set; }
    }
}
