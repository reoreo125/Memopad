using NSubstitute;
using Reoreo125.Memopad.Models.Commands;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class OpenFontCommandTests
{
    IDialogService DialogService { get; set; }

    public OpenFontCommandTests()
    {
        DialogService = Substitute.For<IDialogService>();
    }

    [Fact(DisplayName = "【正常系】CanExecute: 常にtrueを返すこと")]
    public void CanExecute_AlwaysReturnsTrue()
    {
        var command = new OpenFontCommand { DialogService = DialogService };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: DialogService.ShowFontが呼ばれること")]
    public void Execute_ShouldCallDialogServiceShowFont()
    {
        var command = new OpenFontCommand { DialogService = DialogService };

        command.Execute(null);

        DialogService.Received(1).ShowFont();
    }
}
