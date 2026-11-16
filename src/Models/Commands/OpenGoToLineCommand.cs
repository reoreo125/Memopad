using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IOpenGoToLineCommand : ICommand {}

public class OpenGoToLineCommand : CommandBase, IOpenGoToLineCommand
{
    [Dependency]
    public IEditorService? EditorService { get; set; }
    [Dependency]
    public IDialogService? DialogService { get; set; }
    [Dependency]
    public ISettingsService? SettingsService { get; set; }

    public override bool CanExecute(object? parameter) => !SettingsService!.Settings.IsWordWrap.Value;

    public override void Execute(object? parameter)
    {
        RequireInjected(EditorService);
        RequireInjected(DialogService);
        RequireInjected(SettingsService);

        if (!CanExecute(null)) return;

        DialogService.ShowGoToLine(EditorService.Document.Row.Value);
    }
}
