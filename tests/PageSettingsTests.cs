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
}
