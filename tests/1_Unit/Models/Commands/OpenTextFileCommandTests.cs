using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using IDialogService = Reoreo125.Memopad.Models.IDialogService; // Memopad.Models.IDialogService を使用

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class OpenTextFileCommandTests
{
    IDialogService MemopadDialogService { get; set; }
    IEditorService EditorService { get; set; }
    ISettingsService SettingsService { get; set; }
    IEditorDocument Document { get; set; }
    ISettings Settings { get; set; }

    public OpenTextFileCommandTests()
    {
        MemopadDialogService = Substitute.For<IDialogService>();
        EditorService = Substitute.For<IEditorService>();
        SettingsService = Substitute.For<ISettingsService>();

        Document = Substitute.For<IEditorDocument>();
        Document.IsDirty.Returns(new ReactiveProperty<bool>(false));
        Document.FilePath.Returns(new ReactiveProperty<string>(""));
        Document.FileNameWithoutExtension.Returns(new ReactiveProperty<string>("Untitled"));
        EditorService.Document.Returns(Document);

        Settings = Substitute.For<ISettings>();
        Settings.LastOpenedFolderPath.Returns(new ReactiveProperty<string>("C:\\"));
        SettingsService.Settings.Returns(Settings);
    }

    [Fact(DisplayName = "【正常系】CanExecute: 常にtrueを返すこと")]
    public void CanExecute_AlwaysReturnsTrue()
    {
        var command = new OpenTextFileCommand
        {
            EditorService = EditorService,
            MemopadDialogService = MemopadDialogService,
            SettingsService = SettingsService
        };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】未保存の変更がなく、ShowOpenFileでOKの場合、LoadTextが呼ばれること")]
    public void Execute_NoDirtyAndOpenOk_ShouldCallLoadText()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(false));

        var openDialogResult = new DialogResult(ButtonResult.OK);
        openDialogResult.Parameters.Add("filename", @"C:\\temp\\existingfile.txt");
        MemopadDialogService.ShowOpenFile(Arg.Any<string>()).Returns(openDialogResult);

        var command = new OpenTextFileCommand
        {
            EditorService = EditorService,
            MemopadDialogService = MemopadDialogService,
            SettingsService = SettingsService
        };

        command.Execute(null);

        MemopadDialogService.DidNotReceiveWithAnyArgs().ShowConfirmSave(string.Empty);
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
        MemopadDialogService.Received(1).ShowOpenFile(Arg.Any<string>());
        EditorService.Received(1).LoadText(@"C:\\temp\\existingfile.txt");
    }

    [Fact(DisplayName = "【正常系】未保存変更あり、ConfirmSaveでYes、ShowSaveFileでOK、ShowOpenFileでOKの場合、SaveTextとLoadTextが呼ばれること")]
    public void Execute_DirtyConfirmYesSaveOkOpenOk_ShouldCallSaveTextAndLoadText()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        Document.FilePath.Returns(new ReactiveProperty<string>(""));

        MemopadDialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Yes));
        var saveDialogResult = new DialogResult(ButtonResult.OK);
        saveDialogResult.Parameters.Add("filename", @"C:\\temp\\newfile.txt");
        MemopadDialogService.ShowSaveFile().Returns(saveDialogResult);
        var openDialogResult = new DialogResult(ButtonResult.OK);
        openDialogResult.Parameters.Add("filename", @"C:\\temp\\existingfile.txt");
        MemopadDialogService.ShowOpenFile(Arg.Any<string>()).Returns(openDialogResult);

        var command = new OpenTextFileCommand
        {
            EditorService = EditorService,
            MemopadDialogService = MemopadDialogService,
            SettingsService = SettingsService
        };

        command.Execute(null);

        MemopadDialogService.Received(1).ShowConfirmSave(Arg.Any<string>());
        MemopadDialogService.Received(1).ShowSaveFile();
        EditorService.Received(1).SaveText(@"C:\\temp\\newfile.txt");
        MemopadDialogService.Received(1).ShowOpenFile(Arg.Any<string>());
        EditorService.Received(1).LoadText(@"C:\\temp\\existingfile.txt");
    }

    [Fact(DisplayName = "【正常系】未保存変更あり、ConfirmSaveでNo、ShowOpenFileでOKの場合、SaveTextは呼ばれずにLoadTextが呼ばれること")]
    public void Execute_DirtyConfirmNoOpenOk_ShouldCallLoadTextWithoutSaving()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));

        MemopadDialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.No));
        var openDialogResult = new DialogResult(ButtonResult.OK);
        openDialogResult.Parameters.Add("filename", @"C:\\temp\\existingfile.txt");
        MemopadDialogService.ShowOpenFile(Arg.Any<string>()).Returns(openDialogResult);

        var command = new OpenTextFileCommand
        {
            EditorService = EditorService,
            MemopadDialogService = MemopadDialogService,
            SettingsService = SettingsService
        };

        command.Execute(null);

        MemopadDialogService.Received(1).ShowConfirmSave(Arg.Any<string>());
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
        MemopadDialogService.Received(1).ShowOpenFile(Arg.Any<string>());
        EditorService.Received(1).LoadText(@"C:\\temp\\existingfile.txt");
    }

    [Fact(DisplayName = "【正常系】未保存変更あり、ConfirmSaveでCancelの場合、何もせずに終了すること")]
    public void Execute_DirtyConfirmCancel_ShouldExitWithoutAnyAction()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));

        MemopadDialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Cancel));

        var command = new OpenTextFileCommand
        {
            EditorService = EditorService,
            MemopadDialogService = MemopadDialogService,
            SettingsService = SettingsService
        };

        command.Execute(null);

        MemopadDialogService.Received(1).ShowConfirmSave(Arg.Any<string>());
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
        MemopadDialogService.DidNotReceiveWithAnyArgs().ShowOpenFile(string.Empty);
        EditorService.DidNotReceiveWithAnyArgs().LoadText(string.Empty);
    }

    [Fact(DisplayName = "【正常系】未保存変更あり、ConfirmSaveでYes、ShowSaveFileでCancelの場合、何もせずに終了すること")]
    public void Execute_DirtyConfirmYesSaveCancel_ShouldExitWithoutAnyAction()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        Document.FilePath.Returns(new ReactiveProperty<string>(""));

        MemopadDialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Yes));
        MemopadDialogService.ShowSaveFile().Returns(new DialogResult(ButtonResult.Cancel));

        var command = new OpenTextFileCommand
        {
            EditorService = EditorService,
            MemopadDialogService = MemopadDialogService,
            SettingsService = SettingsService
        };

        command.Execute(null);

        MemopadDialogService.Received(1).ShowConfirmSave(Arg.Any<string>());
        MemopadDialogService.Received(1).ShowSaveFile();
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
        MemopadDialogService.DidNotReceiveWithAnyArgs().ShowOpenFile(string.Empty);
        EditorService.DidNotReceiveWithAnyArgs().LoadText(string.Empty);
    }

    [Fact(DisplayName = "【正常系】ShowOpenFileでCancelの場合、LoadTextは呼ばれずに終了すること")]
    public void Execute_OpenFileDialogCancel_ShouldExitWithoutLoading()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(false));

        MemopadDialogService.ShowOpenFile(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Cancel));

        var command = new OpenTextFileCommand
        {
            EditorService = EditorService,
            MemopadDialogService = MemopadDialogService,
            SettingsService = SettingsService
        };

        command.Execute(null);

        MemopadDialogService.DidNotReceiveWithAnyArgs().ShowConfirmSave(string.Empty);
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
        MemopadDialogService.Received(1).ShowOpenFile(Arg.Any<string>());
        EditorService.DidNotReceiveWithAnyArgs().LoadText(string.Empty);
    }
}
