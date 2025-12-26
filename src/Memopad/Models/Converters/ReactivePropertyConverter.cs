using Newtonsoft.Json;
using R3;

namespace Reoreo125.Memopad.Models.Converters;

public class ReactivePropertyConverter : JsonConverter
{
    // ReactiveProperty<> 型を対象にする
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsGenericType &&
               (objectType.GetGenericTypeDefinition() == typeof(ReactiveProperty<>) ||
                objectType.GetGenericTypeDefinition() == typeof(BindableReactiveProperty<>));
    }

    // 書き出し：Valueプロパティの中身だけを書く
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var prop = value!.GetType().GetProperty("Value");
        serializer.Serialize(writer, prop!.GetValue(value));
    }

    
    // 読み込み：JSONの値を使って新しいReactivePropertyを作る
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var valueType = objectType.GetGenericArguments()[0];
        var value = serializer.Deserialize(reader, valueType);

        // リフレクションで new ReactiveProperty<T>(value) を実行
        return Activator.CreateInstance(objectType, value)!;
    }
}
