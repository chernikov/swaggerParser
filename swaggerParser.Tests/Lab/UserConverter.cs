using Newtonsoft.Json;
using System;

namespace swaggerParser.Tests.Lab
{
    public class UserConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            User user = (User)value;

            writer.WriteValue(user.UserName);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            User user = new User
            {
                UserName = (string)reader.Value
            };

            return user;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(User);
        }
    }
}
