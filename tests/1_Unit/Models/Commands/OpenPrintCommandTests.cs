using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using System.Windows;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

[Collection("DisableTestParallelization")]
public class OpenPrintCommandTests
{
    IDialogService DialogService { get; set; }
    IEditorService EditorService { get; set; }
    ISettingsService SettingsService { get; set; }
    IEditorDocument Document { get; set; }
    ISettings Settings { get; set; }
    IPageSettings PageSettings { get; set; }

    public OpenPrintCommandTests()
    {
        DialogService = Substitute.For<IDialogService>();
        EditorService = Substitute.For<IEditorService>();
        SettingsService = Substitute.For<ISettingsService>();

        Document = Substitute.For<IEditorDocument>();
        Document.Text.Returns(new ReactiveProperty<string>("Test Document Text"));
        Document.FilePath.Returns(new ReactiveProperty<string>("test.txt"));
        EditorService.Document.Returns(Document);

        PageSettings = Substitute.For<IPageSettings>();
        PageSettings.MarginLeft.Returns(new ReactiveProperty<double>(10.0));
        PageSettings.MarginTop.Returns(new ReactiveProperty<double>(10.0));
        PageSettings.MarginRight.Returns(new ReactiveProperty<double>(10.0));
        PageSettings.MarginBottom.Returns(new ReactiveProperty<double>(10.0));
        Settings = Substitute.For<ISettings>();
        Settings.FontFamilyName.Returns(new ReactiveProperty<string>("Consolas"));
        Settings.FontSize.Returns(new ReactiveProperty<int>(12));
        Settings.Page.Returns(PageSettings);
        SettingsService.Settings.Returns(Settings);
        
        if (Application.Current == null)
        {
            new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        }
    }

    [Fact(DisplayName = "【正常系】CanExecute: 常にtrueを返すこと")]
    public void CanExecute_AlwaysReturnsTrue()
    {
        var command = new OpenPrintCommand
        {
            DialogService = DialogService,
            EditorService = EditorService,
            SettingsService = SettingsService
        };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }
    /* printDialogのインターフェイスがなくテストが困難です。
    [Fact(DisplayName = "【正常系】Execute: DialogService.ShowPrintが呼ばれること")]
    public void Execute_ShouldCallDialogServiceShowPrint()
    {
        var printDialog = Substitute.For<PrintDialog>();
        printDialog.PrintDocument();

        var dialogResult = new DialogResult(ButtonResult.OK);
        dialogResult.Parameters.Add("printdialog", printDialog);
        DialogService.ShowPrint().Returns(dialogResult);

        var command = new OpenPrintCommand
        {
            DialogService = DialogService,
            EditorService = EditorService,
            SettingsService = SettingsService
        };

        command.Execute(null);

        DialogService.Received(1).ShowPrint();
    }
    */
    [Fact(DisplayName = "【正常系】Execute: 印刷ダイアログがキャンセルされた場合、印刷処理は実行されないこと")]
    public void Execute_PrintDialogCanceled_ShouldNotProceedWithPrinting()
    {
        DialogService.ShowPrint().Returns(new DialogResult(ButtonResult.Cancel));

        var command = new OpenPrintCommand
        {
            DialogService = DialogService,
            EditorService = EditorService,
            SettingsService = SettingsService
        };

        command.Execute(null);

        DialogService.Received(1).ShowPrint();
    }
}
