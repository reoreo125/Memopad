using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class FindNextCommandTests
{
    IDialogService DialogService { get; set; }
    IEditorService EditorService { get; set; }
    IEditorDocument Document { get; set; }

    public FindNextCommandTests()
    {
        DialogService = Substitute.For<IDialogService>();
        EditorService = Substitute.For<IEditorService>();
        Document = Substitute.For<IEditorDocument>();
        Document.SearchText.Returns(new ReactiveProperty<string>(""));
        EditorService.Document.Returns(Document);
    }

    [Fact(DisplayName = "【正常系】CanExecute: SearchTextが空の場合、falseを返すこと")]
    public void CanExecute_SearchTextIsEmpty_ShouldReturnFalse()
    {
        Document.SearchText.Value = "";
        var command = new FindNextCommand { DialogService = DialogService, EditorService = EditorService };

        var canExecute = command.CanExecute(null);

        Assert.False(canExecute);
    }
    
    [Fact(DisplayName = "【正常系】CanExecute: SearchTextが空でない場合、trueを返すこと")]
    public void CanExecute_SearchTextIsNotEmpty_ShouldReturnTrue()
    {
        Document.SearchText.Value = "test";
        var command = new FindNextCommand { DialogService = DialogService, EditorService = EditorService };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: FindNextがtrueを返す場合、ShowNotFoundは呼ばれないこと")]
    public void Execute_FindNextReturnsTrue_ShouldNotCallShowNotFound()
    {
        EditorService.FindNext().Returns(true);
        var command = new FindNextCommand { DialogService = DialogService, EditorService = EditorService };

        command.Execute(null);

        EditorService.Received(1).FindNext();
        DialogService.DidNotReceiveWithAnyArgs().ShowNotFound(string.Empty);
    }
    
    [Fact(DisplayName = "【正常系】Execute: FindNextがfalseを返す場合、ShowNotFoundが呼ばれること")]
    public void Execute_FindNextReturnsFalse_ShouldCallShowNotFound()
    {
        Document.SearchText.Value = "test";
        EditorService.FindNext().Returns(false);
        var command = new FindNextCommand { DialogService = DialogService, EditorService = EditorService };

        command.Execute(null);

        EditorService.Received(1).FindNext();
        DialogService.Received(1).ShowNotFound("test");
    }
}
