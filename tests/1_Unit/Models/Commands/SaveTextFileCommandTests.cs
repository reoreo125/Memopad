using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class SaveTextFileCommandTests
{
    IDialogService DialogService { get; set; }
    IEditorService EditorService { get; set; }
    ISaveAsTextFileCommand SaveAsTextFileCommand { get; set; }
    IEditorDocument Document { get; set; }

    public SaveTextFileCommandTests()
    {
        DialogService = Substitute.For<IDialogService>();
        EditorService = Substitute.For<IEditorService>();
        SaveAsTextFileCommand = Substitute.For<ISaveAsTextFileCommand>();

        Document = Substitute.For<IEditorDocument>();
        Document.FilePath.Returns(new ReactiveProperty<string>(""));
        
        EditorService.Document.Returns(Document);
    }

    [Fact(DisplayName = "【正常系】CanExecute: 常にtrueを返すこと")]
    public void CanExecute_AlwaysReturnsTrue()
    {
        var command = new SaveTextFileCommand
        {
            DialogService = DialogService,
            EditorService = EditorService,
            SaveAsTextFileCommand = SaveAsTextFileCommand
        };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】FilePathが設定済みの場合、EditorService.SaveTextが呼ばれること")]
    public void Execute_FilePathIsSet_ShouldCallSaveText()
    {
        Document.FilePath.Value = @"C:\temp\existingfile.txt"; // ファイルパス設定済み

        var command = new SaveTextFileCommand
        {
            DialogService = DialogService,
            EditorService = EditorService,
            SaveAsTextFileCommand = SaveAsTextFileCommand
        };

        command.Execute(null);

        EditorService.Received(1).SaveText();
        SaveAsTextFileCommand.DidNotReceiveWithAnyArgs().Execute(default);
    }

    [Fact(DisplayName = "【正常系】FilePathが設定されていない場合、SaveAsTextFileCommand.Executeが呼ばれること")]
    public void Execute_FilePathIsNotSet_ShouldCallSaveAsTextFileCommand()
    {
        Document.FilePath.Value = "";

        var command = new SaveTextFileCommand
        {
            DialogService = DialogService,
            EditorService = EditorService,
            SaveAsTextFileCommand = SaveAsTextFileCommand
        };

        command.Execute(null);

        SaveAsTextFileCommand.Received(1).Execute(null);
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
    }
}
