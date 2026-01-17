using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using System.Windows;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

[Collection("DisableTestParallelization")]
public class ExitCommandTests
{
    IEditorService EditorService { get; set; }
    IDialogService DialogService { get; set; }
    ISaveAsTextFileCommand SaveAsTextFileCommand { get; set; }
    IEditorDocument Document { get; set; }

    public ExitCommandTests()
    {
        if (Application.Current is null)
        {
            new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        }

        EditorService = Substitute.For<IEditorService>();
        DialogService = Substitute.For<IDialogService>();
        SaveAsTextFileCommand = Substitute.For<ISaveAsTextFileCommand>();

        Document = Substitute.For<IEditorDocument>();
        Document.IsDirty.Returns(new ReactiveProperty<bool>(false));
        Document.FilePath.Returns(new ReactiveProperty<string>(""));
        Document.FileNameWithoutExtension.Returns(new ReactiveProperty<string>("Untitled"));
        
        EditorService.Document.Returns(Document);
    }
    [Fact(DisplayName = "【正常系】Execute:未保存の変更がない場合、ConfirmSaveを呼び出さないこと")]
    public void Execute_NoDirty_ShouldNotCallConfirmSave()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(false));

        var command = new ExitCommand
        {
            EditorService = EditorService,
            DialogService = DialogService,
            SaveAsTextFileCommand = SaveAsTextFileCommand
        };

        command.Execute(null);

        DialogService.DidNotReceiveWithAnyArgs().ShowConfirmSave(string.Empty);
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
    }
    
    [Fact(DisplayName = "【正常系】Execute:未保存の変更があり、ConfirmSaveでYesを選択した場合、ShowSaveFileとSaveTextが呼ばれること")]
    public void Execute_DirtyAndConfirmYes_ShouldCallShowSaveFileAndSaveText()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        Document.FilePath.Returns(new ReactiveProperty<string>(""));

        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Yes));
        var saveDialogResult = new DialogResult(ButtonResult.OK);
        saveDialogResult.Parameters.Add("filename", @"C:\temp\newfile.txt");
        DialogService.ShowSaveFile().Returns(saveDialogResult);

        var command = new ExitCommand
        {
            EditorService = EditorService,
            DialogService = DialogService,
            SaveAsTextFileCommand = SaveAsTextFileCommand
        };

        command.Execute(null);

        DialogService.Received(1).ShowConfirmSave(Arg.Any<string>());
        DialogService.Received(1).ShowSaveFile();
        EditorService.Received(1).SaveText(@"C:\temp\newfile.txt");
    }
    
    [Fact(DisplayName = "【正常系】Execute:未保存の変更があり、ConfirmSaveでNoを選択した場合、SaveTextを呼び出さないこと")]
    public void Execute_DirtyAndConfirmNo_ShouldNotCallSaveText()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));

        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.No));

        var command = new ExitCommand
        {
            EditorService = EditorService,
            DialogService = DialogService,
            SaveAsTextFileCommand = SaveAsTextFileCommand
        };

        command.Execute(null);

        DialogService.Received(1).ShowConfirmSave(Arg.Any<string>());
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
        DialogService.DidNotReceiveWithAnyArgs().ShowSaveFile();
    }
    
    [Fact(DisplayName = "【正常系】Execute:未保存の変更があり、ConfirmSaveでCancelを選択した場合、何もせずに終了すること")]
    public void Execute_DirtyAndConfirmCancel_ShouldExitWithoutAnyAction()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));

        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Cancel));

        var command = new ExitCommand
        {
            EditorService = EditorService,
            DialogService = DialogService,
            SaveAsTextFileCommand = SaveAsTextFileCommand
        };

        command.Execute(null);

        DialogService.Received(1).ShowConfirmSave(Arg.Any<string>());
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
        DialogService.DidNotReceiveWithAnyArgs().ShowSaveFile();
    }
    
    [Fact(DisplayName = "【正常系】Execute:未保存の変更があり、SaveFileでCancelを選択した場合、SaveTextを呼び出さずに終了すること")]
    public void Execute_DirtyAndSaveFileCancel_ShouldExitWithoutSaveText()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        Document.FilePath.Returns(new ReactiveProperty<string>(""));

        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Yes));
        DialogService.ShowSaveFile().Returns(new DialogResult(ButtonResult.Cancel));

        var command = new ExitCommand
        {
            EditorService = EditorService,
            DialogService = DialogService,
            SaveAsTextFileCommand = SaveAsTextFileCommand
        };

        command.Execute(null);

        DialogService.Received(1).ShowConfirmSave(Arg.Any<string>());
        DialogService.Received(1).ShowSaveFile();
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
    }
}
