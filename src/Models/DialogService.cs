using System.Printing;
using System.Windows.Controls;
using Microsoft.Win32;
using Prism.Dialogs;
using Reoreo125.Memopad.Views.Dialogs;
using Reoreo125.Memopad.Views.Windows;
using IPrismDialogService = Prism.Dialogs.IDialogService;
using PrismDialogService = Prism.Dialogs.DialogService;
namespace Reoreo125.Memopad.Models;

public interface IDialogService
{
    public IDialogResult? ConfirmSave(string fileNameWithoutExtension);
    public string? ShowOpenFile(string folderPath);
    public string? ShowSaveFile();
    public  (bool?, PrintDialog) ShowPrint();
    public IDialogResult? ShowFind();
    public IDialogResult? ShowNotFound(string searchText);
    public IDialogResult? ShowAbout();
    public IDialogResult? ShowGoToLine(int currentLineIndex);
    public IDialogResult? ShowLineOutOfBounds();
    public IDialogResult? ShowReplace();
    public IDialogResult? ShowPageSettings();
    public IDialogResult? ShowFont();
    public IDialogResult? ShowFontNotFound(string message);
}
public class DialogService : IDialogService
{
    [Dependency]
    public IPrismDialogService? PrismDialogService { get; set; }
    [Dependency]
    public ISettingsService SettingsService { get; set;  }

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
    public string? ShowOpenFile(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath)) folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        var openFileDialog = new OpenFileDialog
        {
            Title = "ファイルを開く",
            Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
            InitialDirectory = folderPath
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
        if (SettingsService is null) throw new Exception(nameof(SettingsService));

        PrintDialog printDialog = new PrintDialog();

        PrintTicket ticket = printDialog.PrintQueue.DefaultPrintTicket;
        ticket.PageOrientation = SettingsService.Settings.Page.Orientation.Value;
        ticket.PageMediaSize = new PageMediaSize(SettingsService.Settings.Page.PaperSizeName.Value);
        ticket.InputBin = SettingsService.Settings.Page.InputBin.Value;
        printDialog.PrintTicket = ticket;

        return (printDialog.ShowDialog(), printDialog);
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
    public IDialogResult? ShowGoToLine(int currentLineIndex)
    {
        var parameters = new DialogParameters
        {
            { "lineIndex", currentLineIndex },
        };
        IDialogResult? result = null;
        PrismDialogService.ShowDialog(
            nameof(GoToLineDialog),
            parameters,
            _result => result = _result);

        return result;
    }
    public IDialogResult? ShowFontNotFound(string message)
    {
        var parameters = new DialogParameters
        {
            { "message", message },
        };
        IDialogResult? result = null;
        PrismDialogService.ShowDialog(
            nameof(FontNotFoundDialog),
            parameters,
            _result => result = _result);

        return result;
    }

    // ShowWithoutParameters
    public IDialogResult? ShowFind() => ShowWithoutParameters(typeof(FindDialog));

    // ShowDialogWithoutParameters
    public IDialogResult? ShowLineOutOfBounds() => ShowDialogWithoutParameters(typeof(LineOutOfBoundsDialog));
    public IDialogResult? ShowReplace() => ShowWithoutParameters(typeof(ReplaceDialog));
    public IDialogResult? ShowAbout() => ShowDialogWithoutParameters(typeof(AboutDialog));
    public IDialogResult? ShowPageSettings() => ShowDialogWithoutParameters(typeof(PageSettingsDialog));
    public IDialogResult? ShowFont() => ShowDialogWithoutParameters(typeof(FontDialog));
    
    private IDialogResult? ShowDialogWithoutParameters(Type dialogType)
    {
        IDialogResult? result = null;
        PrismDialogService.ShowDialog(
            dialogType.Name,
            new DialogParameters(),
            _result => result = _result);

        return result;
    }
    private IDialogResult? ShowWithoutParameters(Type dialogType)
    {
        IDialogResult? result = null;
        PrismDialogService.Show(
            dialogType.Name,
            new DialogParameters(),
            _result => result = _result);

        return result;
    }
}
