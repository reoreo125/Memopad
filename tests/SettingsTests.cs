using Reoreo125.Memopad.Models;

namespace Reoreo125.Memopad.Tests
{
    public class SettingsTests
    {
        [Theory]
        [InlineData(@"c:\")]
        public void LastOpenedFolderPath_ValidValue_ShouldKeepValue(string validPath)
        {
            var settings = new Settings();
            settings.LastOpenedFolderPath.Value = validPath;

            var settingsService = new SettingsService();
            settingsService.Validate(settings);

            Assert.Equal(settings.LastOpenedFolderPath.Value, validPath);
        }

        [Theory]
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
