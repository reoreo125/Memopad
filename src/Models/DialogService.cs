using System.Windows.Controls;
using Microsoft.Win32;
using Reoreo125.Memopad.Views.Dialogs;
using Reoreo125.Memopad.Views.Windows;
using IPrismDialogService = Prism.Dialogs.IDialogService;
using PrismDialogService = Prism.Dialogs.DialogService;
namespace Reoreo125.Memopad.Models;

public interface IDialogService
{
    public IDialogResult? ConfirmSave(string fileNameWithoutExtension);
    public string? ShowOpenFile();
    public string? ShowSaveFile();
    public  (bool?, PrintDialog) ShowPrint();
    public IDialogResult? ShowFind();
    public IDialogResult? ShowNotFound(string searchText);
    public IDialogResult? ShowAbout();
    public IDialogResult? ShowGoToLine();
}
public class DialogService : IDialogService
{
    [Dependency]
    public IPrismDialogService? PrismDialogService { get; set; }

    public IDialogResult? ConfirmSave(string fileNameWithoutExtension)
    {
        var parameters = new DialogParameters
        {
            { "title", "Memopad" },
            { "message", $"{fileNameWithoutExtension} への変更内容を保存しますか？" }
        };

        IDialogResult? result = null;
        PrismDialogService.ShowDialog(
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
    public (bool?, PrintDialog) ShowPrint()
    {
        PrintDialog printDialog = new PrintDialog();
        return (printDialog.ShowDialog(), printDialog);
    }
    public IDialogResult? ShowFind()
    {
        IDialogResult? result = null;
        PrismDialogService.Show(
            nameof(FindDialog),
            new DialogParameters(),
            _result => result = _result);

        return result;
    }
    public IDialogResult? ShowNotFound(string searchText)
    {
        var parameters = new DialogParameters
        {
            { "title", "Memopad" },
            { "message", $"\"{searchText}\" が見つかりません。" }
        };

        IDialogResult? result = null;
        PrismDialogService.ShowDialog(
            nameof(NotFoundDialog),
            parameters,
            _result => result = _result);

        return result;
    }
    public IDialogResult? ShowAbout()
    {
        IDialogResult? result = null;
        PrismDialogService.ShowDialog(
            nameof(AboutDialog),
            new DialogParameters(),
            _result => result = _result);

        return result;
    }
    public IDialogResult? ShowGoToLine()
    {
        IDialogResult? result = null;
        PrismDialogService.ShowDialog(
            nameof(GoToLineDialog),
            new DialogParameters(),
            _result => result = _result);

        return result;
    }
}
