using System.Printing;
using System.Windows.Controls;
using Reoreo125.Memopad.Models;

namespace Reoreo125.Memopad.Tests;

public class PageSettingsTests
{

    #region PaperSizeName
    [Theory(DisplayName = "【正常系】PaperSizeName:有効な用紙サイズの場合、値が維持されること")]
    [InlineData(PageMediaSizeName.ISOA4)]
    [InlineData(PageMediaSizeName.NorthAmericaLetter)]
    [InlineData(PageMediaSizeName.JISB4)]
    public void PaperSizeName_ValidValue_ShouldKeepValue(PageMediaSizeName validPaperSizeName)
    {
        var settings = new PageSettings();
        settings.PaperSizeName.Value = validPaperSizeName;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.PaperSizeName.Value, validPaperSizeName);
    }

    [Fact(DisplayName = "【異常系】PaperSizeName:無効な用紙サイズの場合、デフォルト値に復元されること")]
    public void PaperSizeName_InvalidValue_ShouldFallbackToDefault()
    {
        var settings = new PageSettings();
        settings.PaperSizeName.Value = (PageMediaSizeName)99999;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.PaperSizeName.Value, Defaults.PaperSizeName);
    }
    #endregion

    #region InputBin
    [Theory(DisplayName = "【正常系】InputBin:有効な給紙方法の場合、値が維持されること")]
    [InlineData(InputBin.AutoSelect)]
    [InlineData(InputBin.Manual)]
    [InlineData(InputBin.AutoSheetFeeder)]
    public void InputBin_ValidValue_ShouldKeepValue(InputBin validInputBin)
    {
        var settings = new PageSettings();
        settings.InputBin.Value = validInputBin;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.InputBin.Value, validInputBin);
    }

    [Fact(DisplayName = "【異常系】InputBin:無効な給紙方法の場合、デフォルト値に復元されること")]
    public void InputBin_InvalidValue_ShouldFallbackToDefault()
    {
        var settings = new PageSettings();
        settings.InputBin.Value = (InputBin)99999;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.InputBin.Value, Defaults.InputBin);
    }
    #endregion

    #region Orientation
    [Theory(DisplayName = "【正常系】Orientation:有効なページ方向の場合、値が維持されること")]
    [InlineData(PageOrientation.Portrait)]
    [InlineData(PageOrientation.Landscape)]
    public void Orientation_ValidValue_ShouldKeepValue(PageOrientation validPageOrientation)
    {
        var settings = new PageSettings();
        settings.Orientation.Value = validPageOrientation;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.Orientation.Value, validPageOrientation);
    }

    [Fact(DisplayName = "【異常系】Orientation:無効なページ方向の場合、デフォルト値に復元されること")]
    public void Orientation_InvalidValue_ShouldFallbackToDefault()
    {
        var settings = new PageSettings();
        settings.Orientation.Value = (PageOrientation)99999;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.Orientation.Value, Defaults.PageOrientation);
    }
    #endregion
}
