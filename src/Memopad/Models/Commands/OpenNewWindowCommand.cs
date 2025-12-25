using System.Diagnostics;
using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IOpenNewWindowCommand : ICommand
{
}

public class OpenNewWindowCommand : CommandBase, IOpenNewWindowCommand
{
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        // 新しいプロセスとして起動
        Process.Start(new ProcessStartInfo
        {
            FileName = Environment.ProcessPath!,
            UseShellExecute = true // .NET Core/5以降で必要
        });
    }
}
