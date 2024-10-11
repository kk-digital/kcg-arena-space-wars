using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace GameInput
{
    public class InputCommandConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(InputCommand).IsAssignableFrom(objectType);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            JObject jo = JObject.Load(reader);
            string? typeName = jo["$type"]?.ToString();
            if (string.IsNullOrEmpty(typeName)) throw new JsonSerializationException("No $type property found");

            Type? type = Type.GetType(typeName);
            if (type == null) throw new JsonSerializationException($"Type {typeName} not found");

            var result = Activator.CreateInstance(type);
            serializer.Populate(jo.CreateReader(), result!);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            JObject jo = new JObject();
            Type type = value.GetType();
            jo.Add("$type", type.AssemblyQualifiedName);

            foreach (var prop in type.GetProperties())
            {
                if (prop.CanRead && prop.GetMethod?.IsPublic == true)
                {
                    var propValue = prop.GetValue(value);
                    jo.Add(prop.Name, JToken.FromObject(propValue!, serializer));
                }
            }

            jo.WriteTo(writer);
        }
    }
}
