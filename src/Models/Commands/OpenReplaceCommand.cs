using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Reoreo125.Memopad.Models.Commands;

public interface IOpenReplaceCommand : ICommand {}

public class OpenReplaceCommand : CommandBase, IOpenReplaceCommand
{
    [Dependency]
    public IDialogService? DialogService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (DialogService is null) throw new Exception(nameof(DialogService));

        var result = DialogService.ShowReplace();
    }
}
