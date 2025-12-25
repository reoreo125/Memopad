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
    public IMemopadCoreService? MemopadCoreService { get; set; }
    [Dependency]
    public IMemopadDialogService? MemopadDialogService { get; set; }
    [Dependency]
    public ISaveTextFileCommand? SaveTextFileCommand { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (MemopadCoreService is null) throw new Exception(nameof(MemopadCoreService));
        if (MemopadDialogService is null) throw new Exception(nameof(MemopadDialogService));
        if (SaveTextFileCommand is null) throw new Exception(nameof(SaveTextFileCommand));

        var save = false;
        if (MemopadCoreService.IsDirty.Value)
        {
            IDialogResult? result = MemopadDialogService.ConfirmSave(MemopadCoreService.FileNameWithoutExtension.Value);
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

        if (save && string.IsNullOrEmpty(MemopadCoreService.FilePath.Value))
        {
            var saveFilePath = MemopadDialogService.ShowSaveFile();
            if (saveFilePath is null) return;

            MemopadCoreService.FilePath.Value = saveFilePath;
        }

        if (save) MemopadCoreService.SaveText(MemopadCoreService.FilePath.Value);
        MemopadCoreService.Initialize();
    }
}
