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

        var save = false;
        if (EditorService.Document.IsDirty.CurrentValue)
        {
            IDialogResult? result = DialogService.ShowConfirmSave(EditorService.Document.FileNameWithoutExtension.CurrentValue);
            if (result is null) return;

            if (result.Result is ButtonResult.Yes)
            {
                save = true;
            }
            else if (result.Result is ButtonResult.No) { }
            else
            {
                return;
            }
        }

        if (save && string.IsNullOrEmpty(EditorService.Document.FilePath.Value))
        {
            var saveFiledialogResult = DialogService.ShowSaveFile();

            if (saveFiledialogResult is null) return;
            if (saveFiledialogResult.Result is not ButtonResult.OK) return;

            EditorService.Document.FilePath.Value = saveFiledialogResult.Parameters.GetValue<string>("filename");
        }

        if (save) EditorService.SaveText(EditorService.Document.FilePath.Value);
        Application.Current.Shutdown();
    }
}
