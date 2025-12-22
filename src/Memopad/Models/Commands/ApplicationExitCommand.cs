using System.Windows;
using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public class ApplicationExitCommand : CommandBase
{
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        Application.Current.Shutdown();
    }
}
