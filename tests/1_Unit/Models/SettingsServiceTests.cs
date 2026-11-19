using Reoreo125.Memopad.Models;

namespace Reoreo125.Memopad.Tests.Unit.Models;

public class SettingsServiceTests : IDisposable
{
    private readonly string _settingsPath;

    public SettingsServiceTests()
    {
        // SettingsService と同じ保存先をテスト間で共有しないよう、開始前に削除する
        var assemblyName = typeof(SettingsService).Assembly.GetName().Name;
        _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{assemblyName}.settings");
        DeleteSettingsFile();
    }

    public void Dispose() => DeleteSettingsFile();

    private void DeleteSettingsFile()
    {
        if (File.Exists(_settingsPath))
        {
            File.Delete(_settingsPath);
        }
    }

    #region Save, Load
    [Fact(DisplayName = "【正常系】Save/Load:設定を保存し、正しく読み込めること")]
    public void SaveAndLoad_ShouldPreserveSettings()
    {
        using var settingsService = new SettingsService();
        settingsService.Settings.FontSize.Value = 20;
        settingsService.Settings.IsWordWrap.Value = true;
        settingsService.Settings.Page.MarginLeft.Value = 15.5;

        settingsService.Save();
        Settings result = settingsService.Load();

        Assert.Equal(settingsService.Settings.FontSize.Value, result.FontSize.Value);
        Assert.Equal(settingsService.Settings.IsWordWrap.Value, result.IsWordWrap.Value);
        Assert.Equal(settingsService.Settings.Page.MarginLeft.Value, result.Page.MarginLeft.Value);
    }
    
    [Fact(DisplayName = "【異常系】Load:不正なJSONファイルからでもデフォルト設定で起動すること")]
    public void Load_WithInvalidJson_ShouldReturnDefaultSettings()
    {
        using var settingsService = new SettingsService();
        settingsService.Settings.FontSize.Value = 22;
        settingsService.Settings.FontFamilyName.Value = "DummyFontFamilyName";
        File.WriteAllText(settingsService.SettingsPath, "this is not a valid json");

        Settings result = settingsService.Load();

        var defaultSettings = new Settings();
        Assert.Equal(defaultSettings.FontSize.Value, result.FontSize.Value);
        Assert.Equal(defaultSettings.FontFamilyName.Value, result.FontFamilyName.Value);
    }
    #endregion

    #region Dispose
    [Fact(DisplayName = "【正常系】Dispose:Debounce待ちの変更が即時に保存されること")]
    public void Dispose_ShouldSavePendingChangesImmediately()
    {
        var settingsService = new SettingsService();
        settingsService.Settings.FontSize.Value = Defaults.FontSize + 8;
        settingsService.Settings.IsWordWrap.Value = !Defaults.IsWrapping;

        // Debounce(既定5秒)の発火を待たずに破棄する
        settingsService.Dispose();

        using var reloadedService = new SettingsService();
        Assert.Equal(Defaults.FontSize + 8, reloadedService.Settings.FontSize.Value);
        Assert.Equal(!Defaults.IsWrapping, reloadedService.Settings.IsWordWrap.Value);
    }
    #endregion

    #region Validate
    [Fact(DisplayName = "【異常系】Validate:範囲外の値がデフォルト値に修正されること")]
    public void Validate_WithOutOfRangeValue_ShouldFallback()
    {
        var settings = new Settings();
        settings.FontSize.Value = Defaults.FontSizeMax + 1; // 範囲外の値
        using var settingsService = new SettingsService();

        settingsService.Validate(settings);

        Assert.Equal(Defaults.FontSize, settings.FontSize.Value);
    }
    
    [Fact(DisplayName = "【異常系】Validate:不正なフォルダパスがデフォルト値に修正されること")]
    public void Validate_WithInvalidFolderPath_ShouldFallback()
    {
        var settings = new Settings();
        settings.LastOpenedFolderPath.Value = @"C:\invalid\path\that\does\not\exist"; // 不正なパス
        using var settingsService = new SettingsService();

        settingsService.Validate(settings);

        Assert.Equal(Defaults.LastOpenedFolderPath, settings.LastOpenedFolderPath.Value);
    }
    
    [Fact(DisplayName = "【異常系】Validate:ネストしたオブジェクト内の不正な値がデフォルト値に修正されること")]
    public void Validate_WithNestedInvalidValue_ShouldFallback()
    {
        var settings = new Settings();
        settings.Page.MarginBottom.Value = Defaults.MarginMax + 1; // 範囲外
        using var settingsService = new SettingsService();

        settingsService.Validate(settings);

        Assert.Equal(Defaults.MarginBottom, settings.Page.MarginBottom.Value);
    }
    #endregion
}
