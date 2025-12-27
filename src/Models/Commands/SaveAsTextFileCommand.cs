using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface ISaveAsTextFileCommand : ICommand { }
public class SaveAsTextFileCommand : CommandBase, ISaveAsTextFileCommand
{
    [Dependency]
    public IEditorService? EditorService { get; set; }
    [Dependency]
    public IDialogService? DialogService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (EditorService is null) throw new Exception(nameof(EditorService));
        if (DialogService is null) throw new Exception(nameof(DialogService));

        var saveFilePath = DialogService.ShowSaveFile();
        if (string.IsNullOrEmpty(saveFilePath)) return;

        EditorService.SaveText(saveFilePath);
    }
}
