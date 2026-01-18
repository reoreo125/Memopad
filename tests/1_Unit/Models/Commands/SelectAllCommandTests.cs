using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class SelectAllCommandTests
{
    IEditorService EditorService { get; set; }
    IEditorDocument Document { get; set; }

    public SelectAllCommandTests()
    {
        EditorService = Substitute.For<IEditorService>();
        Document = Substitute.For<IEditorDocument>();
        Document.Text.Returns(new ReactiveProperty<string>(""));
        EditorService.Document.Returns(Document);
    }

    [Fact(DisplayName = "【正常系】CanExecute: ドキュメントのテキストが空の場合、falseを返すこと")]
    public void CanExecute_DocumentTextIsEmpty_ShouldReturnFalse()
    {
        Document.Text.Value = "";
        var command = new SelectAllCommand { EditorService = EditorService };

        var canExecute = command.CanExecute(null);

        Assert.False(canExecute);
    }
    
    [Fact(DisplayName = "【正常系】CanExecute: ドキュメントのテキストが空でない場合、trueを返すこと")]
    public void CanExecute_DocumentTextIsNotEmpty_ShouldReturnTrue()
    {
        Document.Text.Value = "some text";
        var command = new SelectAllCommand { EditorService = EditorService };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: CanExecuteがtrueの場合、EditorService.SelectAllが呼ばれること")]
    public void Execute_CanExecuteIsTrue_ShouldCallEditorServiceSelectAll()
    {
        Document.Text.Value = "some text";
        var command = new SelectAllCommand { EditorService = EditorService };

        command.Execute(null);

        EditorService.Received(1).SelectAll();
    }
    
    [Fact(DisplayName = "【正常系】Execute: CanExecuteがfalseの場合、EditorService.SelectAllが呼ばれないこと")]
    public void Execute_CanExecuteIsFalse_ShouldNotCallEditorServiceSelectAll()
    {
        Document.Text.Value = "";
        var command = new SelectAllCommand { EditorService = EditorService };

        command.Execute(null);

        EditorService.DidNotReceive().SelectAll();
    }
}
