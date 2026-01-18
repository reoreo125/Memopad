using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class ZoomCommandTests
{
    ISettingsService SettingsService { get; set; }
    ISettings Settings { get; set; }

    public ZoomCommandTests()
    {
        SettingsService = Substitute.For<ISettingsService>();
        Settings = Substitute.For<ISettings>();
        Settings.ZoomLevel.Returns(new ReactiveProperty<int>(Defaults.ZoomLevel));
        SettingsService.Settings.Returns(Settings);
    }

    [Fact(DisplayName = "【正常系】CanExecute: 常にtrueを返すこと")]
    public void CanExecute_AlwaysReturnsTrue()
    {
        var command = new ZoomCommand { SettingsService = SettingsService };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: 無効なパラメータの場合、ZoomLevelは変更されないこと")]
    public void Execute_InvalidParameter_ShouldNotChangeZoomLevel()
    {
        Settings.ZoomLevel.Value = 100;

        var command = new ZoomCommand { SettingsService = SettingsService };

        command.Execute(null);
        Assert.Equal(100, Settings.ZoomLevel.Value);

        command.Execute("invalid string");
        Assert.Equal(100, Settings.ZoomLevel.Value);
    }

    [Theory(DisplayName = "【正常系】Execute: ZoomOperation.InでZoomLevelが正しく増加すること（最大値でキャップ）")]
    [InlineData(100, 110)]
    [InlineData(495, 500)]
    [InlineData(500, 500)]
    public void Execute_ZoomIn_ShouldIncreaseZoomLevel(int initialZoom, int expectedZoom)
    {
        Settings.ZoomLevel.Value = initialZoom;
        var command = new ZoomCommand { SettingsService = SettingsService };

        command.Execute(ZoomOperation.In);

        Assert.Equal(expectedZoom, Settings.ZoomLevel.Value);
    }

    [Theory(DisplayName = "【正常系】Execute: ZoomOperation.OutでZoomLevelが正しく減少すること（最小値でフロア）")]
    [InlineData(100, 90)]
    [InlineData(15, 10)]
    [InlineData(10, 10)]
    public void Execute_ZoomOut_ShouldDecreaseZoomLevel(int initialZoom, int expectedZoom)
    {
        Settings.ZoomLevel.Value = initialZoom;
        var command = new ZoomCommand { SettingsService = SettingsService };

        command.Execute(ZoomOperation.Out);

        Assert.Equal(expectedZoom, Settings.ZoomLevel.Value);
    }

    [Fact(DisplayName = "【正常系】Execute: ZoomOperation.ResetでZoomLevelがデフォルト値になること")]
    public void Execute_ZoomReset_ShouldSetToDefaultZoomLevel()
    {
        Settings.ZoomLevel.Value = 200;
        var command = new ZoomCommand { SettingsService = SettingsService };

        command.Execute(ZoomOperation.Reset);

        Assert.Equal(Defaults.ZoomLevel, Settings.ZoomLevel.Value);
    }
}
