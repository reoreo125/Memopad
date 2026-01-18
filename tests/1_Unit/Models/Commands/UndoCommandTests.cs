using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class UndoCommandTests
{
    IEditorService EditorService { get; set; }
    IEditorDocument Document { get; set; }

    public UndoCommandTests()
    {
        EditorService = Substitute.For<IEditorService>();
        Document = Substitute.For<IEditorDocument>();
        Document.CanUndo.Returns(new ReactiveProperty<bool>(false));
        EditorService.Document.Returns(Document);
    }

    [Fact(DisplayName = "【正常系】CanExecute: CanUndoがtrueの場合、trueを返すこと")]
    public void CanExecute_CanUndoIsTrue_ShouldReturnTrue()
    {
        Document.CanUndo.Value = true;
        var command = new UndoCommand { EditorService = EditorService };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】CanExecute: CanUndoがfalseの場合、falseを返すこと")]
    public void CanExecute_CanUndoIsFalse_ShouldReturnFalse()
    {
        Document.CanUndo.Value = false;
        var command = new UndoCommand { EditorService = EditorService };

        var canExecute = command.CanExecute(null);

        Assert.False(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: CanExecuteがtrueの場合、EditorService.Undoが呼ばれること")]
    public void Execute_CanExecuteIsTrue_ShouldCallEditorServiceUndo()
    {
        Document.CanUndo.Value = true;
        var command = new UndoCommand { EditorService = EditorService };

        command.Execute(null);

        EditorService.Received(1).Undo();
    }

    [Fact(DisplayName = "【正常系】Execute: CanExecuteがfalseの場合、EditorService.Undoが呼ばれないこと")]
    public void Execute_CanExecuteIsFalse_ShouldNotCallEditorServiceUndo()
    {
        Document.CanUndo.Value = false;
        var command = new UndoCommand { EditorService = EditorService };

        command.Execute(null);

        EditorService.DidNotReceive().Undo();
    }
}
