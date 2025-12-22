using System.Windows;
using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IApplicationExitCommand : ICommand {}

public class ApplicationExitCommand : CommandBase, IApplicationExitCommand
{
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        Application.Current.Shutdown();
    }
}
