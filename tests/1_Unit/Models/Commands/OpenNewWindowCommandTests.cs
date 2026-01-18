using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class OpenNewWindowCommandTests
{
    [Fact(DisplayName = "【正常系】CanExecute: 常にtrueを返すこと")]
    public void CanExecute_AlwaysReturnsTrue()
    {
        var command = new OpenNewWindowCommand();

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    /* このテストは実行しません。
    [Fact(DisplayName = "【正常系】Execute: 例外を投げずに実行が完了すること")]
    public void Execute_ShouldCompleteWithoutException()
    {
        var command = new OpenNewWindowCommand();

        command.Execute(null);
    }
    */
}
