using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class NewTextFileCommandTests
{
    IEditorService EditorService { get; set; }
    IDialogService DialogService { get; set; }
    IEditorDocument Document { get; set; }

    public NewTextFileCommandTests()
    {
        EditorService = Substitute.For<IEditorService>();
        DialogService = Substitute.For<IDialogService>();

        Document = Substitute.For<IEditorDocument>();
        EditorService.Document.Returns(Document);
    }

    [Fact(DisplayName = "【正常系】未保存の変更がない場合、ConfirmSaveを呼び出さずにResetが呼ばれること")]
    public void NewTextFileCommand_NoDirty_ShouldCallReset()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(false));

        var command = new NewTextFileCommand
        {
            EditorService = EditorService,
            DialogService = DialogService,
            SaveTextFileCommand = new SaveTextFileCommand()
        };

        command.Execute(null);

        DialogService.DidNotReceiveWithAnyArgs().ShowConfirmSave(string.Empty);
        EditorService.Received(1).Reset();
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
    }
    
    [Fact(DisplayName = "【正常系】未保存の変更があり、ConfirmSaveでYesを選択した場合、ShowSaveFileとSaveTextとResetが呼ばれること")]
    public void NewTextFileCommand_DirtyAndConfirmYes_ShouldCallShowSaveFileSaveTextAndReset()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        Document.FilePath.Returns(new ReactiveProperty<string>(string.Empty));
        Document.FileNameWithoutExtension.Returns(new ReactiveProperty<string>("newfile"));

        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Yes));
        var saveDialogResult = new DialogResult(ButtonResult.OK);
        saveDialogResult.Parameters.Add("filename", @"C:\temp\newfile.txt");
        DialogService.ShowSaveFile().Returns(saveDialogResult);

        var command = new NewTextFileCommand
        {
            EditorService = EditorService,
            DialogService = DialogService,
            SaveTextFileCommand = new SaveTextFileCommand()
        };

        command.Execute(null);

        DialogService.Received(1).ShowConfirmSave(Arg.Any<string>());
        DialogService.Received(1).ShowSaveFile();
        EditorService.Received(1).SaveText(@"C:\temp\newfile.txt");
        EditorService.Received(1).Reset();
    }
    
    [Fact(DisplayName = "【正常系】未保存の変更があり、ConfirmSaveでNoを選択した場合、SaveTextを呼び出さずにResetが呼ばれること")]
    public void NewTextFileCommand_DirtyAndConfirmNo_ShouldCallResetWithoutSaving()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        Document.FileNameWithoutExtension.Returns(new ReactiveProperty<string>("newfile"));

        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.No));

        var command = new NewTextFileCommand
        {
            EditorService = EditorService,
            DialogService = DialogService,
            SaveTextFileCommand = new SaveTextFileCommand()
        };

        command.Execute(null);

        DialogService.Received(1).ShowConfirmSave(Arg.Any<string>());
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
        DialogService.DidNotReceiveWithAnyArgs().ShowSaveFile();
        EditorService.Received(1).Reset();
    }
    
    [Fact(DisplayName = "【正常系】未保存の変更があり、ConfirmSaveでCancelを選択した場合、何もせずに終了すること")]
    public void NewTextFileCommand_DirtyAndConfirmCancel_ShouldExitWithoutAnyAction()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        Document.FileNameWithoutExtension.Returns(new ReactiveProperty<string>("newfile"));

        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Cancel));

        var command = new NewTextFileCommand
        {
            EditorService = EditorService,
            DialogService = DialogService,
            SaveTextFileCommand = new SaveTextFileCommand()
        };

        command.Execute(null);

        DialogService.Received(1).ShowConfirmSave(Arg.Any<string>());
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
        DialogService.DidNotReceiveWithAnyArgs().ShowSaveFile();
        EditorService.DidNotReceiveWithAnyArgs().Reset();
    }

    [Fact(DisplayName = "【正常系】未保存の変更があり、SaveFileでCancelを選択した場合、SaveTextとResetを呼び出さずに終了すること")]
    public void Execute_DirtyAndSaveFileCancel_ShouldExitWithoutSaveTextAndReset()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        Document.FilePath.Returns(new ReactiveProperty<string>(string.Empty));
        Document.FileNameWithoutExtension.Returns(new ReactiveProperty<string>("newfile"));

        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Yes));
        DialogService.ShowSaveFile().Returns(new DialogResult(ButtonResult.Cancel));

        var command = new NewTextFileCommand
        {
            EditorService = EditorService,
            DialogService = DialogService,
            SaveTextFileCommand = new SaveTextFileCommand()
        };

        command.Execute(null);

        DialogService.Received(1).ShowConfirmSave(Arg.Any<string>());
        DialogService.Received(1).ShowSaveFile();
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
        EditorService.DidNotReceiveWithAnyArgs().Reset();
    }
}
