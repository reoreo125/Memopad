using Newtonsoft.Json;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Converters;

namespace Reoreo125.Memopad.Tests;

public class SerializationTests
{
    [Fact(DisplayName = "【正常系】設定をJSONに変換して復元したとき、全ての値が一致すること")]
    public void Settings_Serialization_ShouldRestoreAllValues()
    {
        var original = new Settings();
        original.FontSize.Value = 42;
        original.FontFamilyName.Value = "Arial";
        original.LastOpenedFolderPath.Value = @"C:\TestPath";
        original.Page.MarginLeft.Value = 15.5;
        original.Page.PaperSizeName.Value = System.Printing.PageMediaSizeName.ISOA4;

        var jsonSettings = new JsonSerializerSettings();
        jsonSettings.Converters.Add(new ReactivePropertyConverter());
        jsonSettings.Formatting = Formatting.Indented;
        string json = JsonConvert.SerializeObject(original, Formatting.Indented, jsonSettings);

        var restored = new Settings();
        JsonConvert.PopulateObject(json, restored, jsonSettings);

        Assert.NotNull(restored);
        Assert.Equal(original.FontSize.Value, restored.FontSize.Value);
        Assert.Equal(original.FontFamilyName.Value, restored.FontFamilyName.Value);
        Assert.Equal(original.LastOpenedFolderPath.Value, restored.LastOpenedFolderPath.Value);

        Assert.NotNull(restored.Page);
        Assert.Equal(original.Page.MarginLeft.Value, restored.Page.MarginLeft.Value);
        Assert.Equal(original.Page.PaperSizeName.Value, restored.Page.PaperSizeName.Value);
    }
}
