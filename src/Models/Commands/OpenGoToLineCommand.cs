using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IOpenGoToLineCommand : ICommand {}

public class OpenGoToLineCommand : CommandBase, IOpenGoToLineCommand
{
    [Dependency]
    public IDialogService? DialogService { get; set; }
    [Dependency]
    public ISettingsService? SettingsService { get; set; }

    public override bool CanExecute(object? parameter) => !SettingsService!.Settings.IsWordWrap.Value;

    public override void Execute(object? parameter)
    {
        if (DialogService is null) throw new Exception(nameof(DialogService));
        if (SettingsService is null) throw new Exception(nameof(SettingsService));

        if (!CanExecute(null)) return;

        DialogService.ShowGoToLine();
    }
}
