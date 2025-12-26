using Newtonsoft.Json;
using R3;

namespace Reoreo125.Memopad.Models.Converters;

public class ReactivePropertyConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsGenericType &&
               (objectType.GetGenericTypeDefinition() == typeof(ReactiveProperty<>) ||
                objectType.GetGenericTypeDefinition() == typeof(BindableReactiveProperty<>));
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var prop = value!.GetType().GetProperty("Value");
        serializer.Serialize(writer, prop!.GetValue(value));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var valueType = objectType.GetGenericArguments()[0];
        var value = serializer.Deserialize(reader, valueType);

        return Activator.CreateInstance(objectType, value)!;
    }
}
