using System.Printing;
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

    #region MarginLeft
    [Theory(DisplayName = "【正常系】MarginLeft:左余白が有効な場合、値が維持されること")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(1000)]
    public void MarginLeft_ValidValue_ShouldKeepValue(double validMarginLeft)
    {
        var settings = new PageSettings();
        settings.MarginLeft.Value = validMarginLeft;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.MarginLeft.Value, validMarginLeft);
    }

    [Theory(DisplayName = "【異常系】MarginLeft:左余白が無効な場合、デフォルト値に復元されること")]
    [InlineData(-1)]
    [InlineData(1001)]
    public void MarginLeft_InvalidValue_ShouldFallbackToDefault(double invalidMarginLeft)
    {
        var settings = new PageSettings();
        settings.MarginLeft.Value = invalidMarginLeft;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.MarginLeft.Value, Defaults.MarginLeft);
    }
    #endregion

    #region MarginTop
    [Theory(DisplayName = "【正常系】MarginTop:上余白が有効な場合、値が維持されること")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(1000)]
    public void MarginTop_ValidValue_ShouldKeepValue(double validMarginTop)
    {
        var settings = new PageSettings();
        settings.MarginTop.Value = validMarginTop;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.MarginTop.Value, validMarginTop);
    }

    [Theory(DisplayName = "【異常系】MarginTop:上余白が無効な場合、デフォルト値に復元されること")]
    [InlineData(-1)]
    [InlineData(1001)]
    public void MarginTop_InvalidValue_ShouldFallbackToDefault(double invalidMarginTop)
    {
        var settings = new PageSettings();
        settings.MarginTop.Value = invalidMarginTop;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.MarginTop.Value, Defaults.MarginTop);
    }
    #endregion

    #region MarginRight
    [Theory(DisplayName = "【正常系】MarginRight:右余白が有効な場合、値が維持されること")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(1000)]
    public void MarginRight_ValidValue_ShouldKeepValue(double validMarginRight)
    {
        var settings = new PageSettings();
        settings.MarginRight.Value = validMarginRight;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.MarginRight.Value, validMarginRight);
    }

    [Theory(DisplayName = "【異常系】MarginRight:右余白が無効な場合、デフォルト値に復元されること")]
    [InlineData(-1)]
    [InlineData(1001)]
    public void MarginRight_InvalidValue_ShouldFallbackToDefault(double invalidMarginRight)
    {
        var settings = new PageSettings();
        settings.MarginRight.Value = invalidMarginRight;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.MarginRight.Value, Defaults.MarginRight);
    }
    #endregion

    #region MarginBottom
    [Theory(DisplayName = "【正常系】MarginBottom:下余白が有効な場合、値が維持されること")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(1000)]
    public void MarginBottom_ValidValue_ShouldKeepValue(double validMarginBottom)
    {
        var settings = new PageSettings();
        settings.MarginBottom.Value = validMarginBottom;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.MarginBottom.Value, validMarginBottom);
    }

    [Theory(DisplayName = "【異常系】MarginBottom:下余白が無効な場合、デフォルト値に復元されること")]
    [InlineData(-1)]
    [InlineData(1001)]
    public void MarginBottom_InvalidValue_ShouldFallbackToDefault(double invalidMarginBottom)
    {
        var settings = new PageSettings();
        settings.MarginBottom.Value = invalidMarginBottom;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.MarginBottom.Value, Defaults.MarginBottom);
    }
    #endregion

    #region Header
    [Theory(DisplayName = "【正常系】Header:ヘッダーが有効な場合、値が維持されること")]
    [InlineData("")]
    [InlineData("TestHeader")]
    [InlineData(@"c:\")]
    public void Header_ValidValue_ShouldKeepValue(string validHeader)
    {
        var settings = new PageSettings();
        settings.Header.Value = validHeader;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.Header.Value, validHeader);
    }

    [Theory(DisplayName = "【異常系】Header:ヘッダーが無効な場合、デフォルト値に復元されること")]
    [InlineData("\n")]
    [InlineData("TestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTest")]
    public void Header_InvalidValue_ShouldFallbackToDefault(string invalidHeader)
    {
        var settings = new PageSettings();
        settings.Header.Value = invalidHeader;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.Header.Value, Defaults.Header);
    }
    #endregion

    #region Footer
    [Theory(DisplayName = "【正常系】Footer:フッターが有効な場合、値が維持されること")]
    [InlineData("")]
    [InlineData("TestFooter")]
    [InlineData(@"c:\")]
    public void Footer_ValidValue_ShouldKeepValue(string validFooter)
    {
        var settings = new PageSettings();
        settings.Footer.Value = validFooter;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.Footer.Value, validFooter);
    }

    [Theory(DisplayName = "【異常系】Footer:フッターが無効な場合、デフォルト値に復元されること")]
    [InlineData("\n")]
    [InlineData("TestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTestTest")]
    public void Footer_InvalidValue_ShouldFallbackToDefault(string invalidFooter)
    {
        var settings = new PageSettings();
        settings.Footer.Value = invalidFooter;

        var settingsService = new SettingsService();
        settingsService.Validate(settings);

        Assert.Equal(settings.Footer.Value, Defaults.Footer);
    }
    #endregion
}
