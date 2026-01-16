using System.Windows.Input;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.Models.Commands;

public interface IOpenTextFileCommand : ICommand { }
public class OpenTextFileCommand : CommandBase, IOpenTextFileCommand
{
    [Dependency]
    public IEditorService? EditorService { get; set; }
    [Dependency]
    public ITextFileService? TextFileService { get; set; }
    [Dependency]
    public IDialogService? MemopadDialogService { get; set; }
    [Dependency]
    public ISettingsService? SettingsService { get; set; }
    //[Dependency]
    //public SaveTextFileCommand? SaveTextFileCommand { get; set; }
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (EditorService is null) throw new Exception(nameof(EditorService));
        if (MemopadDialogService is null) throw new Exception(nameof(MemopadDialogService));
        if (SettingsService is null) throw new Exception(nameof(SettingsService));

        var save = false;
        if (EditorService.Document.IsDirty.CurrentValue)
        {
            IDialogResult?  result = MemopadDialogService.ConfirmSave(EditorService.Document.FileNameWithoutExtension.CurrentValue);
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

        if(save && string.IsNullOrEmpty(EditorService.Document.FilePath.Value))
        {
            var saveFiledialogResult = MemopadDialogService.ShowSaveFile();

            if (saveFiledialogResult is null) return;
            if (saveFiledialogResult.Result is not ButtonResult.OK) return;

            EditorService.Document.FilePath.Value = saveFiledialogResult.Parameters.GetValue<string>("filename");
        }

        var lastOpenedFolderPath = SettingsService.Settings.LastOpenedFolderPath.Value;
        var openFilePathResult = MemopadDialogService.ShowOpenFile(lastOpenedFolderPath);

        if (openFilePathResult is null) return;
        if (openFilePathResult.Result is not ButtonResult.OK) return;

        if (save) EditorService.SaveText(EditorService.Document.FilePath.Value);
        EditorService.LoadText(openFilePathResult.Parameters.GetValue<string>("filename"));
    }
}
