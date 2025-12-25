using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Reoreo125.Memopad.Views.Windows;

namespace Reoreo125.Memopad.Models.Commands;

public interface ISaveAsTextFileCommand : ICommand { }
public class SaveAsTextFileCommand : CommandBase, ISaveAsTextFileCommand
{
    [Dependency]
    public ICoreService? MemopadCoreService { get; set; }
    [Dependency]
    public IDialogService? MemopadDialogService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (MemopadCoreService is null) throw new Exception(nameof(MemopadCoreService));
        if (MemopadDialogService is null) throw new Exception(nameof(MemopadDialogService));

        var saveFilePath = MemopadDialogService.ShowSaveFile();
        if (string.IsNullOrEmpty(saveFilePath)) return;

        MemopadCoreService.SaveText(saveFilePath);
    }
}
