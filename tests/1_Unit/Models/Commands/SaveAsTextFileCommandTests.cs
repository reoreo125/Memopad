using NSubstitute;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class SaveAsTextFileCommandTests
{
    IDialogService DialogService { get; set; }
    IEditorService EditorService { get; set; }

    public SaveAsTextFileCommandTests()
    {
        DialogService = Substitute.For<IDialogService>();
        EditorService = Substitute.For<IEditorService>();
    }

    [Fact(DisplayName = "【正常系】CanExecute: 常にtrueを返すこと")]
    public void CanExecute_AlwaysReturnsTrue()
    {
        var command = new SaveAsTextFileCommand
        {
            DialogService = DialogService,
            EditorService = EditorService
        };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: ShowSaveFileがOKを返した場合、SaveTextが呼ばれること")]
    public void Execute_ShowSaveFileReturnsOk_ShouldCallSaveText()
    {
        var saveDialogResult = new DialogResult(ButtonResult.OK);
        saveDialogResult.Parameters.Add("filename", @"C:\temp\newfile.txt");
        DialogService.ShowSaveFile().Returns(saveDialogResult);

        var command = new SaveAsTextFileCommand
        {
            DialogService = DialogService,
            EditorService = EditorService
        };

        command.Execute(null);

        DialogService.Received(1).ShowSaveFile();
        EditorService.Received(1).SaveText(@"C:\temp\newfile.txt");
    }

    [Fact(DisplayName = "【正常系】Execute: ShowSaveFileがCancelを返した場合、SaveTextは呼ばれないこと")]
    public void Execute_ShowSaveFileReturnsCancel_ShouldNotCallSaveText()
    {
        DialogService.ShowSaveFile().Returns(new DialogResult(ButtonResult.Cancel));

        var command = new SaveAsTextFileCommand
        {
            DialogService = DialogService,
            EditorService = EditorService
        };

        command.Execute(null);

        DialogService.Received(1).ShowSaveFile();
        EditorService.DidNotReceiveWithAnyArgs().SaveText(string.Empty);
    }
}
