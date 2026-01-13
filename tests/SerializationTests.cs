using System.Reflection;
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
    [Fact(DisplayName = "【正常系】設定をファイルから読み出して、全ての値が一致すること")]
    public void Settings_Serialization_ShouldLoadAllValues()
    {
        var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Memopad.settings");
        var json = "{\r\n  \"LastOpenedFolderPath\": \"C:\\\\Users\\\\reore\\\\Documents\",\r\n  \"FontFamilyName\": \"Consolas\",\r\n  \"FontStyleName\": \"Regular\",\r\n  \"FontSize\": 24,\r\n  \"IsWordWrap\": false,\r\n  \"ShowStatusBar\": true,\r\n  \"ZoomLevel\": 110,\r\n  \"Page\": {\r\n    \"PaperSizeName\": 8,\r\n    \"InputBin\": 1,\r\n    \"Orientation\": 2,\r\n    \"MarginLeft\": 20.0,\r\n    \"MarginTop\": 25.0,\r\n    \"MarginRight\": 20.0,\r\n    \"MarginBottom\": 25.0,\r\n    \"Header\": \"TestHeader\",\r\n    \"Footer\": \"Page &p\"\r\n  }\r\n}";
        File.WriteAllText(settingsPath, json);

        var service = new SettingsService();
        service.Load();

        Assert.Equal(24, service.Settings.FontSize.Value);
        Assert.Equal(110, service.Settings.ZoomLevel.Value);
        Assert.Equal("TestHeader", service.Settings.Page.Header.Value);
    }
    [Fact(DisplayName = "【異常系】設定をファイルから読み出してJSONが壊れている場合、デフォルト設定が生成されること")]
    public void Settings_BrokenJson_ShouldReturnDefaultSettings()
    {
        var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Memopad.settings");
        var brokenJson = "{ \"FontSize\": \"NotANumber\" ...";
        File.WriteAllText(settingsPath, brokenJson);

        var service = new SettingsService();
        service.Load();

        Assert.Equal(Defaults.FontSize, service.Settings.FontSize.Value);
    }
}
