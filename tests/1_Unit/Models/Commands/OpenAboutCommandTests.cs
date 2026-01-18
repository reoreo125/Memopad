using NSubstitute;
using Reoreo125.Memopad.Models.Commands;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class OpenAboutCommandTests
{
    IDialogService DialogService { get; set; }

    public OpenAboutCommandTests()
    {
        DialogService = Substitute.For<IDialogService>();
    }

    [Fact(DisplayName = "【正常系】CanExecute: 常にtrueを返すこと")]
    public void CanExecute_AlwaysReturnsTrue()
    {
        var command = new OpenAboutCommand { DialogService = DialogService };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: DialogService.ShowAboutが呼ばれること")]
    public void Execute_ShouldCallDialogServiceShowAbout()
    {
        var command = new OpenAboutCommand { DialogService = DialogService };

        command.Execute(null);

        DialogService.Received(1).ShowAbout();
    }
}
