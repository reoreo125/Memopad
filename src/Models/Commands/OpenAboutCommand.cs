using System.Windows;
using System.Windows.Input;
using Reoreo125.Memopad.Views.Windows;

namespace Reoreo125.Memopad.Models.Commands;

public interface IOpenAboutCommand : ICommand { }
public class OpenAboutCommand : CommandBase, IOpenAboutCommand
{
    [Dependency]
    public IDialogService? DialogService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (DialogService is null) throw new Exception(nameof(DialogService));

        var result = DialogService.ShowAbout();
    }
}
