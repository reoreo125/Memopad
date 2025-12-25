using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Reoreo125.Memopad.Models.TextProcessing;
using Reoreo125.Memopad.Views.Dialogs;

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
    //[Dependency]
    //public SaveTextFileCommand? SaveTextFileCommand { get; set; }
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if(EditorService is null) throw new Exception(nameof(EditorService));
        if(MemopadDialogService is null) throw new Exception(nameof(MemopadDialogService));

        var save = false;
        if (EditorService.IsDirty.Value)
        {
            IDialogResult?  result = MemopadDialogService.ConfirmSave(EditorService.FileNameWithoutExtension.Value);
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

        if(save && string.IsNullOrEmpty(EditorService.FilePath.Value))
        {
            var saveFilePath = MemopadDialogService.ShowSaveFile();
            if (saveFilePath is null) return;

            EditorService.FilePath.Value = saveFilePath;
        }

        string? openFilePath = MemopadDialogService.ShowOpenFile();
        if (openFilePath is null) return;

        if (save) EditorService.SaveText(EditorService.FilePath.Value);
        EditorService.LoadText(openFilePath);
    }
}
