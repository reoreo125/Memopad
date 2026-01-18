using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class OpenGoToLineCommandTests
{
    IEditorService EditorService { get; set; }
    IDialogService DialogService { get; set; }
    ISettingsService SettingsService { get; set; }
    IEditorDocument Document { get; set; }
    ISettings Settings { get; set; }

    public OpenGoToLineCommandTests()
    {
        EditorService = Substitute.For<IEditorService>();
        DialogService = Substitute.For<IDialogService>();
        SettingsService = Substitute.For<ISettingsService>();

        Document = Substitute.For<IEditorDocument>();
        Document.Row.Returns(new ReactiveProperty<int>(1));
        EditorService.Document.Returns(Document);

        Settings = Substitute.For<ISettings>();
        Settings.IsWordWrap.Returns(new ReactiveProperty<bool>(false));
        SettingsService.Settings.Returns(Settings);
    }

    [Fact(DisplayName = "【正常系】CanExecute: IsWordWrapがtrueの場合、falseを返すこと")]
    public void CanExecute_IsWordWrapIsTrue_ShouldReturnFalse()
    {
        Settings.IsWordWrap.Value = true;
        var command = new OpenGoToLineCommand
        {
            EditorService = EditorService,
            DialogService = DialogService,
            SettingsService = SettingsService
        };

        var canExecute = command.CanExecute(null);

        Assert.False(canExecute);
    }

    [Fact(DisplayName = "【正常系】CanExecute: IsWordWrapがfalseの場合、trueを返すこと")]
    public void CanExecute_IsWordWrapIsFalse_ShouldReturnTrue()
    {
        Settings.IsWordWrap.Value = false;
        var command = new OpenGoToLineCommand
        {
            EditorService = EditorService,
            DialogService = DialogService,
            SettingsService = SettingsService
        };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: CanExecuteがtrueの場合、DialogService.ShowGoToLineが呼ばれること")]
    public void Execute_CanExecuteIsTrue_ShouldCallShowGoToLine()
    {
        Settings.IsWordWrap.Value = false;
        Document.Row.Value = 5;

        var command = new OpenGoToLineCommand
        {
            EditorService = EditorService,
            DialogService = DialogService,
            SettingsService = SettingsService
        };

        command.Execute(null);

        DialogService.Received(1).ShowGoToLine(5);
    }

    [Fact(DisplayName = "【正常系】Execute: CanExecuteがfalseの場合、DialogService.ShowGoToLineは呼ばれないこと")]
    public void Execute_CanExecuteIsFalse_ShouldNotCallShowGoToLine()
    {
        Settings.IsWordWrap.Value = true;

        var command = new OpenGoToLineCommand
        {
            EditorService = EditorService,
            DialogService = DialogService,
            SettingsService = SettingsService
        };

        command.Execute(null);

        DialogService.DidNotReceive().ShowGoToLine(Arg.Any<int>());
    }
}
