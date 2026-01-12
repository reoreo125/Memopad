using Reoreo125.Memopad.Models;

namespace Reoreo125.Memopad.Tests
{
    public class SettingsTests
    {
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
    }
}
