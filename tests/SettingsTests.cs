using Reoreo125.Memopad.Models;

namespace Reoreo125.Memopad.Tests
{
    public class SettingsTests
    {
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
    }
}
