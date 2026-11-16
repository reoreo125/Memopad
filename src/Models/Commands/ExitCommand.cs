using System.Windows;
using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IExitCommand : ICommand {}

public class ExitCommand : CommandBase, IExitCommand
{
    [Dependency]
    public IEditorService? EditorService { get; set; }
    [Dependency]
    public IDialogService? DialogService { get; set; }
    [Dependency]
    public ISaveAsTextFileCommand? SaveAsTextFileCommand { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (EditorService is null) throw new Exception(nameof(EditorService));
        if (DialogService is null) throw new Exception(nameof(DialogService));
        if (SaveAsTextFileCommand is null) throw new Exception(nameof(SaveAsTextFileCommand));

        if (CommandHelper.TryConfirmSave(EditorService, DialogService, out var save) is false) return;

        if (save) EditorService.SaveText(EditorService.Document.FilePath.Value);
        Application.Current.Shutdown();
    }
}
