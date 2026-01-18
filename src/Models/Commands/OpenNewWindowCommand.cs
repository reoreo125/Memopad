using System.Diagnostics;
using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IOpenNewWindowCommand : ICommand { }

public class OpenNewWindowCommand : CommandBase, IOpenNewWindowCommand
{
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = Environment.ProcessPath!,
            UseShellExecute = true
        });
    }
}
