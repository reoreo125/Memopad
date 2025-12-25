using System.Windows;
using System.Windows.Input;
using R3;
using Reoreo125.Memopad.Models.History;
using Reoreo125.Memopad.Views.Dialogs;

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
        if (EditorService is null) throw new Exception(nameof(EditorService));
        if (DialogService is null) throw new Exception(nameof(DialogService));
        if (SaveTextFileCommand is null) throw new Exception(nameof(SaveTextFileCommand));

        var save = false;
        if (EditorService.IsDirty.Value)
        {
            IDialogResult? result = DialogService.ConfirmSave(EditorService.FileNameWithoutExtension.Value);
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

        if (save && string.IsNullOrEmpty(EditorService.FilePath.Value))
        {
            var saveFilePath = DialogService.ShowSaveFile();
            if (saveFilePath is null) return;

            EditorService.FilePath.Value = saveFilePath;
        }

        if (save) EditorService.SaveText(EditorService.FilePath.Value);
        EditorService.Initialize();
    }
}
