using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class ToggleStatusBarCommandTests
{
    ISettingsService SettingsService { get; set; }
    ISettings Settings { get; set; }

    public ToggleStatusBarCommandTests()
    {
        SettingsService = Substitute.For<ISettingsService>();
        Settings = Substitute.For<ISettings>();
        Settings.ShowStatusBar.Returns(new ReactiveProperty<bool>(true));
        SettingsService.Settings.Returns(Settings);
    }

    [Fact(DisplayName = "【正常系】CanExecute: 常にtrueを返すこと")]
    public void CanExecute_AlwaysReturnsTrue()
    {
        var command = new ToggleStatusBarCommand { SettingsService = SettingsService };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: ShowStatusBarがtrueの場合、falseにトグルされること")]
    public void Execute_ShowStatusBarIsTrue_ShouldToggleToFalse()
    {
        Settings.ShowStatusBar.Value = true;
        var command = new ToggleStatusBarCommand { SettingsService = SettingsService };

        command.Execute(null);

        Assert.False(Settings.ShowStatusBar.Value);
    }

    [Fact(DisplayName = "【正常系】Execute: ShowStatusBarがfalseの場合、trueにトグルされること")]
    public void Execute_ShowStatusBarIsFalse_ShouldToggleToTrue()
    {
        Settings.ShowStatusBar.Value = false;
        var command = new ToggleStatusBarCommand { SettingsService = SettingsService };

        command.Execute(null);

        Assert.True(Settings.ShowStatusBar.Value);
    }
}
