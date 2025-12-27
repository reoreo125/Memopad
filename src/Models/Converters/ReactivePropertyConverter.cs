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
        var newValue = serializer.Deserialize(reader, valueType);

        if (existingValue != null)
        {
            // 新しいインスタンスを作らず、既存のReactiveProperty.Valueを更新する
            var prop = existingValue.GetType().GetProperty("Value");
            prop?.SetValue(existingValue, newValue);
            return existingValue;
        }

        // 既存のインスタンスがない場合のみ新規作成（通常はここを通らない設定にする）
        return Activator.CreateInstance(objectType, newValue)!;
    }
}
