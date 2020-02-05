using Newtonsoft.Json;
using NUnit.Framework;
using swaggerParser.Tests.Lab;

namespace swaggerParser.Tests
{
    public class JsonTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CheckJsonConverter()
        {
            var user = new User
            {
                UserName = @"domain\username"
            };

            string json = JsonConvert.SerializeObject(user, Formatting.Indented);

            Assert.Pass();
        }
    }
}