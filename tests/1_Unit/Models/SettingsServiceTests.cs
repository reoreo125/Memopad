using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Printing;
using System.Windows.Shell;
using Newtonsoft.Json;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.Tests.Unit.Models;

public class SettingsServiceTests
{
    #region Save, Load
    [Fact(DisplayName = "【正常系】Save/Load:設定を保存し、正しく読み込めること")]
    public void SaveAndLoad_ShouldPreserveSettings()
    {
        var settingsService = new SettingsService();
        settingsService.Settings.FontSize.Value = 20;
        settingsService.Settings.IsWordWrap.Value = true;
        settingsService.Settings.Page.MarginLeft.Value = 15.5;

        Settings result;
        try
        {
            settingsService.Save();
            result = settingsService.Load();
        }
        finally
        {
            if (File.Exists(settingsService.SettingsPath))
            {
                File.Delete(settingsService.SettingsPath);
            }
        }

        Assert.Equal(settingsService.Settings.FontSize.Value, result.FontSize.Value);
        Assert.Equal(settingsService.Settings.IsWordWrap.Value, result.IsWordWrap.Value);
        Assert.Equal(settingsService.Settings.Page.MarginLeft.Value, result.Page.MarginLeft.Value);
    }
    
    [Fact(DisplayName = "【異常系】Load:不正なJSONファイルからでもデフォルト設定で起動すること")]
    public void Load_WithInvalidJson_ShouldReturnDefaultSettings()
    {
        var settingsService = new SettingsService();
        settingsService.Settings.FontSize.Value = 22;
        settingsService.Settings.FontFamilyName.Value = "DummyFontFamilyName";
        File.WriteAllText(settingsService.SettingsPath, "this is not a valid json");

        Settings result;
        try
        {
            result = settingsService.Load();
        }
        finally
        {
            if (File.Exists(settingsService.SettingsPath))
            {
                File.Delete(settingsService.SettingsPath);
            }
        }

        var defaultSettings = new Settings();
        Assert.Equal(defaultSettings.FontSize.Value, result.FontSize.Value);
        Assert.Equal(defaultSettings.FontFamilyName.Value, result.FontFamilyName.Value);
    }
    #endregion

    #region Validate
    [Fact(DisplayName = "【異常系】Validate:範囲外の値がデフォルト値に修正されること")]
    public void Validate_WithOutOfRangeValue_ShouldFallback()
    {
        var settings = new Settings();
        settings.FontSize.Value = Defaults.FontSizeMax + 1; // 範囲外の値
        var settingsService = new SettingsService();

        settingsService.Validate(settings);

        Assert.Equal(Defaults.FontSize, settings.FontSize.Value);
    }
    
    [Fact(DisplayName = "【異常系】Validate:不正なフォルダパスがデフォルト値に修正されること")]
    public void Validate_WithInvalidFolderPath_ShouldFallback()
    {
        var settings = new Settings();
        settings.LastOpenedFolderPath.Value = @"C:\invalid\path\that\does\not\exist"; // 不正なパス
        var settingsService = new SettingsService();

        settingsService.Validate(settings);

        Assert.Equal(Defaults.LastOpenedFolderPath, settings.LastOpenedFolderPath.Value);
    }
    
    [Fact(DisplayName = "【異常系】Validate:ネストしたオブジェクト内の不正な値がデフォルト値に修正されること")]
    public void Validate_WithNestedInvalidValue_ShouldFallback()
    {
        var settings = new Settings();
        settings.Page.MarginBottom.Value = Defaults.MarginMax + 1; // 範囲外
        var settingsService = new SettingsService();

        settingsService.Validate(settings);

        Assert.Equal(Defaults.MarginBottom, settings.Page.MarginBottom.Value);
    }
    #endregion
}
