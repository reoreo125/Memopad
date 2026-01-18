using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IOpenFontCommand : ICommand {}

public class OpenFontCommand : CommandBase, IOpenFontCommand
{
    [Dependency]
    public IDialogService? DialogService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (DialogService is null) throw new Exception(nameof(DialogService));

        DialogService.ShowFont();
    }
}
