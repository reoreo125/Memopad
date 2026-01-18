using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class ToggleWordWrapCommandTests
{
    ISettingsService SettingsService { get; set; }
    ISettings Settings { get; set; }

    public ToggleWordWrapCommandTests()
    {
        SettingsService = Substitute.For<ISettingsService>();
        Settings = Substitute.For<ISettings>();
        Settings.IsWordWrap.Returns(new ReactiveProperty<bool>(true));
        SettingsService.Settings.Returns(Settings);
    }

    [Fact(DisplayName = "【正常系】CanExecute: 常にtrueを返すこと")]
    public void CanExecute_AlwaysReturnsTrue()
    {
        var command = new ToggleWordWrapCommand { SettingsService = SettingsService };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: IsWordWrapがtrueの場合、falseにトグルされること")]
    public void Execute_IsWordWrapIsTrue_ShouldToggleToFalse()
    {
        Settings.IsWordWrap.Value = true;
        var command = new ToggleWordWrapCommand { SettingsService = SettingsService };

        command.Execute(null);

        Assert.False(Settings.IsWordWrap.Value);
    }

    [Fact(DisplayName = "【正常系】Execute: IsWordWrapがfalseの場合、trueにトグルされること")]
    public void Execute_IsWordWrapIsFalse_ShouldToggleToTrue()
    {
        Settings.IsWordWrap.Value = false;
        var command = new ToggleWordWrapCommand { SettingsService = SettingsService };

        command.Execute(null);

        Assert.True(Settings.IsWordWrap.Value);
    }
}
