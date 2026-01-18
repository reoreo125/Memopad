using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class RedoCommandTests
{
    IEditorService EditorService { get; set; }
    IEditorDocument Document { get; set; }

    public RedoCommandTests()
    {
        EditorService = Substitute.For<IEditorService>();
        Document = Substitute.For<IEditorDocument>();
        Document.CanRedo.Returns(new ReactiveProperty<bool>(false));
        EditorService.Document.Returns(Document);
    }

    [Fact(DisplayName = "【正常系】CanExecute: CanRedoがtrueの場合、trueを返すこと")]
    public void CanExecute_CanRedoIsTrue_ShouldReturnTrue()
    {
        Document.CanRedo.Value = true;
        var command = new RedoCommand { EditorService = EditorService };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】CanExecute: CanRedoがfalseの場合、falseを返すこと")]
    public void CanExecute_CanRedoIsFalse_ShouldReturnFalse()
    {
        Document.CanRedo.Value = false;
        var command = new RedoCommand { EditorService = EditorService };

        var canExecute = command.CanExecute(null);

        Assert.False(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: CanExecuteがtrueの場合、EditorService.Redoが呼ばれること")]
    public void Execute_CanExecuteIsTrue_ShouldCallEditorServiceRedo()
    {
        Document.CanRedo.Value = true;
        var command = new RedoCommand { EditorService = EditorService };

        command.Execute(null);

        EditorService.Received(1).Redo();
    }

    [Fact(DisplayName = "【正常系】Execute: CanExecuteがfalseの場合、EditorService.Redoが呼ばれないこと")]
    public void Execute_CanExecuteIsFalse_ShouldNotCallEditorServiceRedo()
    {
        Document.CanRedo.Value = false;
        var command = new RedoCommand { EditorService = EditorService };

        command.Execute(null);

        EditorService.DidNotReceive().Redo();
    }
}
