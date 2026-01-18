using NSubstitute;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using System.Windows;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

[Collection("DisableTestParallelization")]
public class PasteCommandTests
{
    IEditorService EditorService { get; set; }

    public PasteCommandTests()
    {
        EditorService = Substitute.For<IEditorService>();
    }

    [StaFact(DisplayName = "【正常系】Execute: EditorService.Pasteが呼ばれること(クリップボードにテキストがある場合)")]
    public void Execute_ShouldCallEditorServicePaste()
    {
        Clipboard.SetText("test");

        var command = new PasteCommand
        {
            EditorService = EditorService
        };

        command.Execute(null);

        EditorService.Received(1).Paste();
        Clipboard.Clear();
    }

    [StaFact(DisplayName = "【正常系】CanExecute: クリップボードにテキストがある場合、trueを返すこと")]
    public void CanExecute_WhenClipboardHasText_ShouldReturnTrue()
    {
        Clipboard.SetText("test text for CanExecute");
        var command = new PasteCommand
        {
            EditorService = EditorService
        };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
        Clipboard.Clear();
    }

    [StaFact(DisplayName = "【正常系】CanExecute: クリップボードが空の場合、falseを返すこと")]
    public void CanExecute_WhenClipboardIsEmpty_ShouldReturnFalse()
    {
        Clipboard.Clear();
        var command = new PasteCommand
        {
            EditorService = EditorService
        };

        var canExecute = command.CanExecute(null);

        Assert.False(canExecute);
        Clipboard.Clear();
    }
}
