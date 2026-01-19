using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class ReplaceAllCommandTests
{
    IDialogService DialogService { get; set; }
    IEditorService EditorService { get; set; }
    IEditorDocument Document { get; set; }

    public ReplaceAllCommandTests()
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
        var command = new ReplaceAllCommand { DialogService = DialogService, EditorService = EditorService };

        var canExecute = command.CanExecute(null);

        Assert.False(canExecute);
    }
    
    [Fact(DisplayName = "【正常系】CanExecute: SearchTextが空でない場合、trueを返すこと")]
    public void CanExecute_SearchTextIsNotEmpty_ShouldReturnTrue()
    {
        Document.SearchText.Value = "test";
        var command = new ReplaceAllCommand { DialogService = DialogService, EditorService = EditorService };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: ReplaceAllがtrueを返す場合、ShowInformationは呼ばれないこと")]
    public void Execute_ReplaceAllReturnsTrue_ShouldNotCallShowInformation()
    {
        EditorService.ReplaceAll().Returns(true);
        var command = new ReplaceAllCommand { DialogService = DialogService, EditorService = EditorService };

        command.Execute(null);

        EditorService.Received(1).ReplaceAll();
        DialogService.DidNotReceiveWithAnyArgs().ShowInformation(string.Empty, string.Empty);
    }
    
    [Fact(DisplayName = "【正常系】Execute: ReplaceAllがfalseを返す場合、ShowInformationが呼ばれること")]
    public void Execute_ReplaceAllReturnsFalse_ShouldCallShowInformation()
    {
        Document.SearchText.Value = "test";
        EditorService.ReplaceAll().Returns(false);
        var command = new ReplaceAllCommand { DialogService = DialogService, EditorService = EditorService };

        command.Execute(null);

        EditorService.Received(1).ReplaceAll();
        DialogService.Received(1).ShowInformation("Memopad", $"\"{Document.SearchText.Value}\" が見つかりません。");
    }
}
