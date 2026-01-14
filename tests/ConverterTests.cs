using Newtonsoft.Json;
using R3;
using Reoreo125.Memopad.Models.Converters;
using Xunit;

namespace Reoreo125.Memopad.Tests;

public class ConverterTests
{
    private readonly JsonSerializerSettings _settings = new()
    {
        Converters = new[] { new ReactivePropertyConverter() },
    };

    #region ReactivePropertyConverter
    [Theory(DisplayName = "【正常系】ReactivePropertyのValueをJSONとして書き込むこと")]
    [InlineData("test string")]
    [InlineData(12345)]
    [InlineData(true)]
    [InlineData(null)]
    public void ReactivePropertyConverter_WriteJson_Test<T>(T value)
    {
        var rp = new ReactiveProperty<T>(value);
        var expectedJson = JsonConvert.SerializeObject(value);

        var actualJson = JsonConvert.SerializeObject(rp, _settings);

        Assert.Equal(expectedJson, actualJson);
    }

    [Theory(DisplayName = "【正常系】JSONからReactivePropertyの新しいインスタンスを生成して読み込むこと")]
    [InlineData("\"test string\"", "test string")]
    [InlineData("12345", 12345)]
    [InlineData("true", true)]
    [InlineData("null", null)]
    public void ReactivePropertyConverter_ReadJson_CreatesNewInstance_Test<T>(string json, T expectedValue)
    {
        var rp = JsonConvert.DeserializeObject<ReactiveProperty<T>>(json, _settings);

        Assert.NotNull(rp);
        Assert.Equal(expectedValue, rp.Value);
    }
    
    [Theory(DisplayName = "【正常系】JSONから既存のReactivePropertyインスタンスのValueを更新すること")]
    [InlineData("\"new string\"", "old string", "new string")]
    [InlineData("999", 111, 999)]
    [InlineData("false", true, false)]
    public void ReactivePropertyConverter_ReadJson_PopulatesExistingInstance_Test<T>(string json, T initialValue, T expectedValue)
    {
        var rp = new ReactiveProperty<T>(initialValue);

        JsonConvert.PopulateObject(json, rp, _settings);

        Assert.Equal(expectedValue, rp.Value);
    }

    [Fact(DisplayName = "【正常系】ReactiveProperty型とBindableReactiveProperty型を正しく変換可能と判断すること")]
    public void ReactivePropertyConverter_CanConvert_Test()
    {
        var converter = new ReactivePropertyConverter();

        Assert.True(converter.CanConvert(typeof(ReactiveProperty<string>)));
        Assert.True(converter.CanConvert(typeof(BindableReactiveProperty<int>)));
        Assert.False(converter.CanConvert(typeof(string)));
        Assert.False(converter.CanConvert(typeof(int)));
    }
    #endregion
}
