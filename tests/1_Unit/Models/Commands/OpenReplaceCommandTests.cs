using NSubstitute;
using Reoreo125.Memopad.Models.Commands;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class OpenReplaceCommandTests
{
    IDialogService DialogService { get; set; }

    public OpenReplaceCommandTests()
    {
        DialogService = Substitute.For<IDialogService>();
    }

    [Fact(DisplayName = "【正常系】CanExecute: 常にtrueを返すこと")]
    public void CanExecute_AlwaysReturnsTrue()
    {
        var command = new OpenReplaceCommand { DialogService = DialogService };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: DialogService.ShowReplaceが呼ばれること")]
    public void Execute_ShouldCallDialogServiceShowReplace()
    {
        var command = new OpenReplaceCommand { DialogService = DialogService };

        command.Execute(null);

        DialogService.Received(1).ShowReplace();
    }
}
