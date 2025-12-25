using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Reoreo125.Memopad.Views.Windows;

namespace Reoreo125.Memopad.Models.Commands;

public interface ISaveTextFileCommand : ICommand { }
public class SaveTextFileCommand : CommandBase, ISaveTextFileCommand
{
    [Dependency]
    public ICoreService? MemopadCoreService { get; set; }
    [Dependency]
    public IDialogService? MemopadDialogService { get; set; }
    [Dependency]
    public ISaveAsTextFileCommand? SaveAsTextFileCommand { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (MemopadCoreService is null) throw new Exception(nameof(MemopadCoreService));
        if (MemopadDialogService is null) throw new Exception(nameof(MemopadDialogService));
        if (SaveAsTextFileCommand is null) throw new Exception(nameof(SaveAsTextFileCommand));

        if (string.IsNullOrEmpty(MemopadCoreService.FilePath.Value))
        {
            SaveAsTextFileCommand.Execute(null);
            return;
        }

        MemopadCoreService.SaveText(MemopadCoreService.FilePath.Value);
    }
}
