using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using Xunit;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class CopyCommandTests
{
    IEditorService EditorService { get; set; }
    IEditorDocument Document { get; set; }

    public CopyCommandTests()
    {
        EditorService = Substitute.For<IEditorService>();
        Document = Substitute.For<IEditorDocument>();
        Document.SelectionLength.Returns(new ReactiveProperty<int>(0));
        EditorService.Document.Returns(Document);
    }

    [Fact(DisplayName = "【正常系】CanExecute: SelectionLength > 0 の場合、trueを返すこと")]
    public void CanExecute_SelectionLengthIsGreaterThanZero_ShouldReturnTrue()
    {
        Document.SelectionLength.Value = 10;
        var command = new CopyCommand { EditorService = EditorService };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】CanExecute: SelectionLength = 0 の場合、falseを返すこと")]
    public void CanExecute_SelectionLengthIsZero_ShouldReturnFalse()
    {
        Document.SelectionLength.Value = 0;
        var command = new CopyCommand { EditorService = EditorService };

        var canExecute = command.CanExecute(null);

        Assert.False(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: CanExecuteがtrueの場合、EditorService.Copyが呼ばれること")]
    public void Execute_CanExecuteIsTrue_ShouldCallEditorServiceCopy()
    {
        Document.SelectionLength.Value = 10;
        var command = new CopyCommand { EditorService = EditorService };

        command.Execute(null);

        EditorService.Received(1).Copy();
    }

    [Fact(DisplayName = "【正常系】Execute: CanExecuteがfalseの場合、EditorService.Copyが呼ばれないこと")]
    public void Execute_CanExecuteIsFalse_ShouldNotCallEditorServiceCopy()
    {
        Document.SelectionLength.Value = 0;
        var command = new CopyCommand { EditorService = EditorService };

        command.Execute(null);

        EditorService.DidNotReceive().Copy();
    }
}
