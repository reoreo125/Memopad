using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Reoreo125.Memopad.Views.Dialogs;

namespace Reoreo125.Memopad.Models.Commands;

public interface IOpenTextFileCommand : ICommand { }
public class OpenTextFileCommand : CommandBase, IOpenTextFileCommand
{
    [Dependency]
    public IMemopadCoreService? MemopadCoreService { get; set; }

    [Dependency]
    public IDialogService? DialogService { get; set; }
    //[Dependency]
    //public SaveTextFileCommand? SaveTextFileCommand { get; set; }
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if(MemopadCoreService is null) throw new Exception("MemopadCoreService");

        IDialogResult? result = null;

        if (MemopadCoreService.IsDirty.Value)
        {
            var parameters = new DialogParameters { { "message", $"{MemopadCoreService.FileNameWithoutExtension.Value} への変更内容を保存しますか？" } };
            DialogService.ShowDialog(
                nameof(SaveConfirmDialog),
                parameters,
                _result => result = _result);
        }

        if (result is null) { }
        else if(result.Result is ButtonResult.Yes)
        {
            //SaveTestFileCommand.Execute(null);
        }
        else if(result.Result is ButtonResult.No) { }
        else
        {
            return;
        }

        var openFileDialog = new OpenFileDialog
        {
            Title = "ファイルを開く",
            Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        if (openFileDialog.ShowDialog() is true)
        {
            string filePath = openFileDialog.FileName;

            if (MemopadCoreService is null) throw new Exception("MemopadCoreService");
            MemopadCoreService.LoadText(filePath);
        }
    }
}
