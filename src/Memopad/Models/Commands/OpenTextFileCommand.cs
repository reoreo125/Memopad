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
    public ICoreService? MemopadCoreService { get; set; }
    [Dependency]
    public ITextFileService? TextFileService { get; set; }
    [Dependency]
    public IDialogService? MemopadDialogService { get; set; }
    //[Dependency]
    //public SaveTextFileCommand? SaveTextFileCommand { get; set; }
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if(MemopadCoreService is null) throw new Exception(nameof(MemopadCoreService));
        if(MemopadDialogService is null) throw new Exception(nameof(MemopadDialogService));

        var save = false;
        if (MemopadCoreService.IsDirty.Value)
        {
            IDialogResult?  result = MemopadDialogService.ConfirmSave(MemopadCoreService.FileNameWithoutExtension.Value);
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

        if(save && string.IsNullOrEmpty(MemopadCoreService.FilePath.Value))
        {
            var saveFilePath = MemopadDialogService.ShowSaveFile();
            if (saveFilePath is null) return;

            MemopadCoreService.FilePath.Value = saveFilePath;
        }

        string? openFilePath = MemopadDialogService.ShowOpenFile();
        if (openFilePath is null) return;

        if (save) MemopadCoreService.SaveText(MemopadCoreService.FilePath.Value);
        MemopadCoreService.LoadText(openFilePath);
    }
}
