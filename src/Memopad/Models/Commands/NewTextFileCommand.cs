using System.Windows;
using System.Windows.Input;
using Reoreo125.Memopad.Models.Services;

namespace Reoreo125.Memopad.Models.Commands;

public interface INewTextFileCommand : ICommand { }
public class NewTextFileCommand : CommandBase, INewTextFileCommand
{
    [Dependency]
    public IMemopadCoreService? MemopadCoreService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (MemopadCoreService is null) throw new Exception("MemopadCoreService");

        if(MemopadCoreService.IsDirty.Value)
        {
            var result = MessageBox.Show(
                "新しいファイルを作成します。現在の内容は保存されません。",
                "確認",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.OK) return;
        }
        

        MemopadCoreService.Initialize();
    }
}
