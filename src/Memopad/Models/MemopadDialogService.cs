using Microsoft.Win32;
using Prism.Dialogs;
using R3;
using Reoreo125.Memopad.Views.Dialogs;

namespace Reoreo125.Memopad.Models;

public interface IMemopadDialogService
{
    IDialogResult? ConfirmSave(string fileNameWithoutExtension);
    string? ShowOpenFile();
    string? ShowSaveFile();
}
public class MemopadDialogService : IMemopadDialogService
{
    [Dependency]
    public IDialogService? DialogService { get; set; }


    public IDialogResult? ConfirmSave(string fileNameWithoutExtension)
    {
        var parameters = new DialogParameters { { "message", $"{fileNameWithoutExtension} への変更内容を保存しますか？" } };

        IDialogResult? result = null;

        DialogService.ShowDialog(
            nameof(SaveConfirmDialog),
            parameters,
            _result => result = _result);

        return result;
    }
    public string? ShowOpenFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "ファイルを開く",
            Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        var result = openFileDialog.ShowDialog();
        if (result is false) return null;

        return openFileDialog.FileName;
    }
    public string? ShowSaveFile()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = "名前を付けて保存",
            Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
            DefaultExt = "txt",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };
        var result = saveFileDialog.ShowDialog();
        if (result is false) return null;

        return saveFileDialog.FileName;
    }
}
