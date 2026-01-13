using R3;
using Reoreo125.Memopad.Models;

namespace Reoreo125.Memopad.Tests;

public class SettingsTests
{
    #region Changed
    [Fact(DisplayName = "【正常系】Changed:プロパティがどれか一つでも変更されると通知されること")]
    public void Changed_AnyPropertyChanged_ShouldNotify()
    {
        var settings = new Settings();
        var result = false;
        using (settings.Changed.Subscribe(_ => result = true))
        {
            settings.FontSize.Value = Defaults.FontSize + 2;
        }

        Assert.True(result);
    }
    [Fact(DisplayName = "【正常系】Changed:プロパティが同じ値で変更されると通知すること")]
    public void Changed_AnyPropertyChangedSameValue_ShouldNotify()
    {
        var settings = new Settings();
        var result = false;
        using (settings.Changed.Subscribe(_ => result = true))
        {
            settings.FontSize.Value = Defaults.FontSize;
        }

        Assert.True(result);
    }
    #endregion

    #region LastOpenedFolderPath
    [Theory(DisplayName = "【正常系】LastOpenedFolderPath:最後に開いたフォルダパスが有効な場合、値が維持されること")]
    [InlineData(@"c:\")]
    [InlineData(@"C:\Users\Public\Documents")]
    public void LastOpenedFolderPath_ValidValue_ShouldKeepValue(string validPath)
    {
        var settings = new Settings();
        settings.LastOpenedFolderPath.Value = validPath;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.LastOpenedFolderPath.Value, validPath);
    }

    [Theory(DisplayName = "【異常系】LastOpenedFolderPath:最後に開いたフォルダパスが無効な場合、デフォルト値に復元されること")]
    [InlineData("")]            // 空文字
    [InlineData("   ")]         // 空白のみ
    [InlineData(@"Z:\Invalid\Path\That\Does\Not\Exist")] // 存在しないパス
    [InlineData("12345")]
    [InlineData("abasdgdfsg")]
    [InlineData("!+:+`P}*")]
    public void LastOpenedFolderPath_InvalidValue_ShouldFallbackToDefault(string invalidPath)
    {
        var settings = new Settings();
        settings.LastOpenedFolderPath.Value = invalidPath;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.LastOpenedFolderPath.Value, Defaults.LastOpenedFolderPath);
    }
    #endregion

    #region FontFamilyName
    [Theory(DisplayName = "【正常系】FontFamilyName:フォントファミリー名が有効な場合、値が維持されること")]
    [InlineData("Arial")]
    [InlineData("Consolas")]
    [InlineData("Times New Roman")]
    [InlineData("MS Gothic")]
    public void FontFamilyName_ValidValue_ShouldKeepValue(string validFontFamilyName)
    {
        var settings = new Settings();
        settings.FontFamilyName.Value = validFontFamilyName;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.FontFamilyName.Value, validFontFamilyName);
    }

    [Theory(DisplayName = "【異常系】FontFamilyName:フォントファミリー名が無効な場合、デフォルト値に復元されること")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("TestFontFamily")]
    public void FontFamilyName_InvalidValue_ShouldFallbackToDefault(string invalidFontFamilyName)
    {
        var settings = new Settings();
        settings.FontFamilyName.Value = invalidFontFamilyName;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.FontFamilyName.Value, Defaults.FontFamilyName);
    }
    #endregion

    #region FontStyleName
    [Theory(DisplayName = "【正常系】FontStyleName:フォントスタイル名が有効な場合、値が維持されること")]
    [InlineData("Regular")]
    [InlineData("Italic")]
    [InlineData("Oblique")]
    [InlineData("Bold")]
    [InlineData("Bold Italic")]
    [InlineData("Bold Oblique")]
    public void FontStyleName_ValidValue_ShouldKeepValue(string validFontStyleName)
    {
        var settingsService = new SettingsService();
        var settings = new Settings();
        settings.FontFamilyName.Value = "Consolas";
        settings.FontStyleName.Value = validFontStyleName;

        settingsService.Validate(settings);

        Assert.Equal(settings.FontStyleName.Value, validFontStyleName);
    }

    [Theory(DisplayName = "【異常系】FontStyleName:フォントスタイル名が無効な場合、デフォルト値(Regular)に復元されること")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("TestStyle")]
    public void FontStyleName_InvalidValue_ShouldFallbackToDefault_1(string invalidFontStyleName)
    {
        var settingsService = new SettingsService();
        var settings = new Settings();
        settings.FontFamilyName.Value = "Consolas";
        settings.FontStyleName.Value = invalidFontStyleName;

        settingsService.Validate(settings);

        //Assert.Equal(settings.FontFamilyName.Value, Defaults.GetFontStyleName(settings.FontFamilyName.Value));
        Assert.Equal("Regular", settings.FontStyleName.Value);
    }

    [Theory(DisplayName = "【異常系】FontStyleName:フォントスタイル名が無効な場合、デフォルト値(Normal)に復元されること")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("TestStyle")]
    public void FontStyleName_InvalidValue_ShouldFallbackToDefault_2(string invalidFontStyleName)
    {
        var settingsService = new SettingsService();
        var settings = new Settings();
        settings.FontFamilyName.Value = "Arial";
        settings.FontStyleName.Value = invalidFontStyleName;

        settingsService.Validate(settings);

        //Assert.Equal(settings.FontFamilyName.Value, Defaults.GetFontStyleName(settings.FontFamilyName.Value));
        Assert.Equal("Normal", settings.FontStyleName.Value);
    }

    [Theory(DisplayName = "【異常系】FontStyleName:フォントスタイル名が無効な場合、デフォルト値(既定外)に復元されること")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("TestStyle")]
    public void FontStyleName_InvalidValue_ShouldFallbackToDefault_3(string invalidFontStyleName)
    {
        var settingsService = new SettingsService();
        var settings = new Settings();
        settings.FontFamilyName.Value = "DejaVu Sans";
        settings.FontStyleName.Value = invalidFontStyleName;

        settingsService.Validate(settings);

        //Assert.Equal(settings.FontFamilyName.Value, Defaults.GetFontStyleName(settings.FontFamilyName.Value));
        Assert.Equal("ExtraLight", settings.FontStyleName.Value);
    }
    #endregion

    #region FontSize
    [Theory(DisplayName = "【正常系】FontSize:フォントサイズが有効な場合、値が維持されること")]
    [InlineData(1)]
    [InlineData(12)]
    [InlineData(256)]
    [InlineData(999)]
    public void FontSize_ValidValue_ShouldKeepValue(int validFontSize)
    {
        var settings = new Settings();
        settings.FontSize.Value = validFontSize;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.FontSize.Value, validFontSize);
    }

    [Theory(DisplayName = "【異常系】FontSize:フォントサイズが無効な場合、デフォルト値に復元されること")]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1000)]
    public void FontSize_InvalidValue_ShouldFallbackToDefault(int invalidFontSize)
    {
        var settings = new Settings();
        settings.FontSize.Value = invalidFontSize;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.FontSize.Value, Defaults.FontSize);
    }
    #endregion

    #region IsWordWrap
    #endregion

    #region ShowStatusBar
    #endregion

    #region ZoomLevel
    [Theory(DisplayName = "【正常系】ZoomLevel:ズームレベルが有効な場合、値が維持されること")]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(250)]
    [InlineData(500)]
    public void ZoomLevel_ValidValue_ShouldKeepValue(int validZoomLevel)
    {
        var settings = new Settings();
        settings.ZoomLevel.Value = validZoomLevel;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.ZoomLevel.Value, validZoomLevel);
    }

    [Theory(DisplayName = "【異常系】ZoomLevel:ズームレベルが無効な場合、デフォルト値に復元されること")]
    [InlineData(-10)]
    [InlineData(0)]
    [InlineData(9)]
    [InlineData(501)]
    public void ZoomLevel_InvalidValue_ShouldFallbackToDefault(int invalidZoomLevel)
    {
        var settings = new Settings();
        settings.ZoomLevel.Value = invalidZoomLevel;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.ZoomLevel.Value, Defaults.ZoomLevel);
    }
    #endregion
}
