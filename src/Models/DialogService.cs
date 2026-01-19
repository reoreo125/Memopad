using System.Printing;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Reoreo125.Memopad.Views.Dialogs;
using IPrismDialogService = Prism.Dialogs.IDialogService;
using PrismDialogService = Prism.Dialogs.DialogService;
namespace Reoreo125.Memopad.Models;

public interface IDialogService
{
    public IDialogResult? ShowConfirmSave(string fileNameWithoutExtension);
    public IDialogResult? ShowOpenFile(string folderPath);
    public IDialogResult? ShowSaveFile();
    public IDialogResult? ShowPrint();
    public IDialogResult? ShowFind();
    public IDialogResult? ShowAbout();
    public IDialogResult? ShowGoToLine(int currentLineIndex);
    public IDialogResult? ShowReplace();
    public IDialogResult? ShowPageSettings();
    public IDialogResult? ShowFont();

    public IDialogResult? ShowInformation(string title, string message);
    public IDialogResult? ShowWarning(string title, string message);
    public IDialogResult? ShowError(string title, string message);
}
public class DialogService : IDialogService
{
    [Dependency]
    public IPrismDialogService? PrismDialogService { get; set; }
    [Dependency]
    public ISettingsService? SettingsService { get; set; }

    public IDialogResult? ShowConfirmSave(string fileNameWithoutExtension)
    {
        var parameters = new DialogParameters
        {
            { "message", $"{fileNameWithoutExtension} への変更内容を保存しますか？" }
        };

        IDialogResult? result = null;
        PrismDialogService.ShowDialog(
            nameof(ConfirmSaveDialog),
            parameters,
            _result => result = _result);

        return result;
    }
    public IDialogResult? ShowOpenFile(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath)) folderPath = Defaults.LastOpenedFolderPath;

        var openFileDialog = new OpenFileDialog
        {
            Title = "ファイルを開く",
            Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
            InitialDirectory = folderPath
        };

        var resultBool = openFileDialog.ShowDialog();

        var dialogResult = new DialogResult
        {
            Parameters = new DialogParameters()
            {
                { "filename", openFileDialog.FileName }
            },
            Result = resultBool is true ? ButtonResult.OK : ButtonResult.Cancel
        };

        return dialogResult;
    }
    public IDialogResult? ShowSaveFile()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = "名前を付けて保存",
            Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
            DefaultExt = "txt",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        var resultBool = saveFileDialog.ShowDialog();

        var dialogResult = new DialogResult
        {
            Parameters = new DialogParameters()
            {
                { "filename", saveFileDialog.FileName }
            },
            Result = resultBool is true ? ButtonResult.OK : ButtonResult.Cancel
        };

        return dialogResult;
    }
    public IDialogResult? ShowPrint()
    {
        if (SettingsService is null) throw new Exception(nameof(SettingsService));

        PrintDialog printDialog = new PrintDialog();

        PrintTicket ticket = printDialog.PrintQueue.DefaultPrintTicket;
        ticket.PageOrientation = SettingsService.Settings.Page.Orientation.Value;
        ticket.PageMediaSize = new PageMediaSize(SettingsService.Settings.Page.PaperSizeName.Value);
        ticket.InputBin = SettingsService.Settings.Page.InputBin.Value;
        printDialog.PrintTicket = ticket;

        var resultBool = printDialog.ShowDialog();

        var dialogResult = new DialogResult
        {
            Parameters = new DialogParameters()
            {
                { "printdialog", printDialog }
            },
            Result = resultBool is true ? ButtonResult.OK : ButtonResult.Cancel
        };

        return dialogResult;
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

    // ShowModelessWithoutParameters
    public IDialogResult? ShowFind() => ShowModelessWithoutParameters(typeof(FindDialog));
    public IDialogResult? ShowReplace() => ShowModelessWithoutParameters(typeof(ReplaceDialog));

    // ShowDialogWithoutParameters
    public IDialogResult? ShowAbout() => ShowModalWithoutParameters(typeof(AboutDialog));
    public IDialogResult? ShowPageSettings() => ShowModalWithoutParameters(typeof(PageSettingsDialog));
    public IDialogResult? ShowFont() => ShowModalWithoutParameters(typeof(FontDialog));

    //ShowModalWithTitleAndMessage
    public IDialogResult? ShowInformation(string title, string message) => ShowModalWithTitleAndMessage(typeof(InformationDialog), title, message);
    public IDialogResult? ShowWarning(string title, string message) => ShowModalWithTitleAndMessage(typeof(WarningDialog), title, message);
    public IDialogResult? ShowError(string title, string message) => ShowModalWithTitleAndMessage(typeof(ErrorDialog), title, message);

    private IDialogResult? ShowModalWithoutParameters(Type dialogType)
    {
        IDialogResult? result = null;
        PrismDialogService.ShowDialog(
            dialogType.Name,
            new DialogParameters(),
            _result => result = _result);

        return result;
    }
    private IDialogResult? ShowModelessWithoutParameters(Type dialogType)
    {
        var existingWindow = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(w => w.GetType().Namespace?.StartsWith("Prism.Dialogs") is true);
        if (existingWindow is not null) existingWindow.Close();
        
        IDialogResult? result = null;
        PrismDialogService.Show(
            dialogType.Name,
            new DialogParameters(),
            _result => result = _result);

        return result;
    }
    public IDialogResult? ShowModalWithTitleAndMessage(Type dialogType, string title, string message)
    {
        var parameters = new DialogParameters
        {
            { "title", title },
            { "message", message }
        };
        IDialogResult? result = null;
        PrismDialogService.ShowDialog(
            dialogType.Name,
            parameters,
            _result => result = _result);
        return result;
    }
}
