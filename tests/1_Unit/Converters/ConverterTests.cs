using Newtonsoft.Json;
using R3;
using Reoreo125.Memopad.Models.Converters;
using Reoreo125.Memopad.Views.Converters;
using System.Globalization;
using System.Printing;
using System.Windows.Data;
using Xunit;

namespace Reoreo125.Memopad.Tests.Unit.Converters;

public class ConverterTests
{
    public class Holder<T>
    {
        public ReactiveProperty<T> Target { get; set; } = new();
    }
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
        var holder = new Holder<T>();
        holder.Target.Value = initialValue;

        var originalInstance = holder.Target;

        JsonConvert.PopulateObject($"{{\"Target\": {json}}}", holder, _settings);

        Assert.Same(originalInstance, holder.Target);
        Assert.Equal(expectedValue, holder.Target.Value);
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

    #region InverseBoolConverter
    [Theory(DisplayName = "【正常系】InverseBoolConverter.Convert: bool値を反転させること")]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void InverseBoolConverter_Convert_ShouldInverseValue(bool input, bool expected)
    {
        var converter = new InverseBoolConverter();
        var result = converter.Convert(input, typeof(bool), null!, CultureInfo.CurrentCulture);
        Assert.Equal(expected, result);
    }

    [Theory(DisplayName = "【正常系】InverseBoolConverter.ConvertBack: bool値を反転させること")]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void InverseBoolConverter_ConvertBack_ShouldInverseValue(bool input, bool expected)
    {
        var converter = new InverseBoolConverter();
        var result = converter.ConvertBack(input, typeof(bool), null!, CultureInfo.CurrentCulture);
        Assert.Equal(expected, result);
    }
    #endregion

    #region PageOrientationToBoolConverter
    [Theory(DisplayName = "【正常系】PageOrientationToBoolConverter.Convert: valueとparameterが一致すればtrueを返す")]
    [InlineData(PageOrientation.Portrait, PageOrientation.Portrait, true)]
    [InlineData(PageOrientation.Portrait, PageOrientation.Landscape, false)]
    [InlineData(PageOrientation.Landscape, PageOrientation.Portrait, false)]
    [InlineData(PageOrientation.Landscape, PageOrientation.Landscape, true)]
    public void PageOrientationToBoolConverter_Convert_ShouldReturnTrueIfValueEqualsParameter(PageOrientation value, PageOrientation parameter, bool expected)
    {
        var converter = new PageOrientationToBoolConverter();
        var result = converter.Convert(value, typeof(bool), parameter, CultureInfo.CurrentCulture);
        Assert.Equal(expected, result);
    }

    [Fact(DisplayName = "【正常系】PageOrientationToBoolConverter.ConvertBack: trueならparameterを返す")]
    public void PageOrientationToBoolConverter_ConvertBack_ShouldReturnParameterIfTrue()
    {
        var converter = new PageOrientationToBoolConverter();
        var parameter = PageOrientation.Portrait;
        var result = converter.ConvertBack(true, typeof(PageOrientation), parameter, CultureInfo.CurrentCulture);
        Assert.Equal(parameter, result);
    }

    [Fact(DisplayName = "【正常系】PageOrientationToBoolConverter.ConvertBack: falseならBinding.DoNothingを返す")]
    public void PageOrientationToBoolConverter_ConvertBack_ShouldReturnDoNothingIfFalse()
    {
        var converter = new PageOrientationToBoolConverter();
        var result = converter.ConvertBack(false, typeof(PageOrientation), PageOrientation.Portrait, CultureInfo.CurrentCulture);
        Assert.Equal(Binding.DoNothing, result);
    }
    #endregion
}
