using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface INewTextFileCommand : ICommand { }
public class NewTextFileCommand : CommandBase, INewTextFileCommand
{
    [Dependency]
    public IEditorService? EditorService { get; set; }
    [Dependency]
    public IDialogService? DialogService { get; set; }
    [Dependency]
    public ISaveTextFileCommand? SaveTextFileCommand { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        RequireInjected(EditorService);
        RequireInjected(DialogService);
        RequireInjected(SaveTextFileCommand);

        if (CommandHelper.TryConfirmSave(EditorService, DialogService, out var save) is false) return;

        if (save) EditorService.SaveText(EditorService.Document.FilePath.Value);
        EditorService.Reset();
    }
}
