using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Reoreo125.Memopad.Views.Windows;

namespace Reoreo125.Memopad.Models.Commands;

public interface ISaveAsTextFileCommand : ICommand { }
public class SaveAsTextFileCommand : CommandBase, ISaveAsTextFileCommand
{
    [Dependency]
    public IMemopadCoreService? MemopadCoreService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (MemopadCoreService is null) throw new Exception("MemopadCoreService");

        var dialog = new SaveFileDialog
        {
            Title = "名前を付けて保存",
            Filter = "テキスト文書 (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
            DefaultExt = "txt",
            // 現在のファイル名があれば初期値としてセット
            FileName = MemopadCoreService.FileName.Value
        };
        var result = dialog.ShowDialog();

        if (result is true)
        {
            string selectedFilePath = dialog.FileName;
            MemopadCoreService.SaveText(selectedFilePath);
        }
    }
}
