using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class CommandHelperTests
{
    IEditorService EditorService { get; }
    IDialogService DialogService { get; }
    IEditorDocument Document { get; }

    public CommandHelperTests()
    {
        EditorService = Substitute.For<IEditorService>();
        DialogService = Substitute.For<IDialogService>();

        Document = Substitute.For<IEditorDocument>();
        Document.IsDirty.Returns(new ReactiveProperty<bool>(false));
        Document.FilePath.Returns(new ReactiveProperty<string>(""));
        Document.FileNameWithoutExtension.Returns(new ReactiveProperty<string>("Untitled"));
        EditorService.Document.Returns(Document);
    }

    #region 変更なし
    [Fact(DisplayName = "【正常系】TryConfirmSave:未保存の変更がない場合、確認せずに続行すること")]
    public void TryConfirmSave_NoDirty_ShouldReturnTrueWithoutDialog()
    {
        var result = CommandHelper.TryConfirmSave(EditorService, DialogService, out var saveRequested);

        Assert.True(result);
        Assert.False(saveRequested);
        DialogService.DidNotReceiveWithAnyArgs().ShowConfirmSave(string.Empty);
        DialogService.DidNotReceiveWithAnyArgs().ShowSaveFile();
    }
    #endregion

    #region 確認ダイアログ
    [Fact(DisplayName = "【異常系】TryConfirmSave:確認ダイアログがnullの場合、falseを返すこと")]
    public void TryConfirmSave_ConfirmDialogReturnsNull_ShouldReturnFalse()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns((IDialogResult?)null);

        var result = CommandHelper.TryConfirmSave(EditorService, DialogService, out var saveRequested);

        Assert.False(result);
        Assert.False(saveRequested);
        DialogService.Received(1).ShowConfirmSave("Untitled");
        DialogService.DidNotReceiveWithAnyArgs().ShowSaveFile();
    }

    [Fact(DisplayName = "【正常系】TryConfirmSave:Yesかつ保存先が設定済みの場合、SaveFileを表示せず保存を要求すること")]
    public void TryConfirmSave_YesWithFilePath_ShouldRequestSaveWithoutSaveFileDialog()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        Document.FilePath.Returns(new ReactiveProperty<string>(@"C:\temp\file.txt"));
        Document.FileNameWithoutExtension.Returns(new ReactiveProperty<string>("file"));
        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Yes));

        var result = CommandHelper.TryConfirmSave(EditorService, DialogService, out var saveRequested);

        Assert.True(result);
        Assert.True(saveRequested);
        DialogService.Received(1).ShowConfirmSave("file");
        DialogService.DidNotReceiveWithAnyArgs().ShowSaveFile();
    }

    [Fact(DisplayName = "【正常系】TryConfirmSave:Noの場合、保存を要求せずに続行すること")]
    public void TryConfirmSave_No_ShouldReturnTrueWithoutSaveRequest()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.No));

        var result = CommandHelper.TryConfirmSave(EditorService, DialogService, out var saveRequested);

        Assert.True(result);
        Assert.False(saveRequested);
        DialogService.DidNotReceiveWithAnyArgs().ShowSaveFile();
    }

    [Fact(DisplayName = "【異常系】TryConfirmSave:Cancelの場合、falseを返すこと")]
    public void TryConfirmSave_Cancel_ShouldReturnFalse()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Cancel));

        var result = CommandHelper.TryConfirmSave(EditorService, DialogService, out var saveRequested);

        Assert.False(result);
        Assert.False(saveRequested);
        DialogService.DidNotReceiveWithAnyArgs().ShowSaveFile();
    }
    #endregion

    #region 保存先ダイアログ
    [Fact(DisplayName = "【異常系】TryConfirmSave:Yesで保存先が未設定、SaveFileがnullの場合、falseを返すこと")]
    public void TryConfirmSave_YesWithoutFilePathAndSaveFileNull_ShouldReturnFalse()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Yes));
        DialogService.ShowSaveFile().Returns((IDialogResult?)null);

        var result = CommandHelper.TryConfirmSave(EditorService, DialogService, out var saveRequested);

        Assert.False(result);
        Assert.True(saveRequested);
    }

    [Fact(DisplayName = "【異常系】TryConfirmSave:Yesで保存先が未設定、SaveFileでCancelの場合、falseを返すこと")]
    public void TryConfirmSave_YesWithoutFilePathAndSaveFileCancel_ShouldReturnFalse()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Yes));
        DialogService.ShowSaveFile().Returns(new DialogResult(ButtonResult.Cancel));

        var result = CommandHelper.TryConfirmSave(EditorService, DialogService, out var saveRequested);

        Assert.False(result);
        Assert.True(saveRequested);
    }

    [Fact(DisplayName = "【正常系】TryConfirmSave:Yesで保存先が未設定、SaveFileでOKの場合、FilePathが更新されること")]
    public void TryConfirmSave_YesWithoutFilePathAndSaveFileOk_ShouldUpdateFilePath()
    {
        Document.IsDirty.Returns(new ReactiveProperty<bool>(true));
        DialogService.ShowConfirmSave(Arg.Any<string>()).Returns(new DialogResult(ButtonResult.Yes));
        var saveDialogResult = new DialogResult(ButtonResult.OK);
        saveDialogResult.Parameters.Add("filename", @"C:\temp\newfile.txt");
        DialogService.ShowSaveFile().Returns(saveDialogResult);

        var result = CommandHelper.TryConfirmSave(EditorService, DialogService, out var saveRequested);

        Assert.True(result);
        Assert.True(saveRequested);
        Assert.Equal(@"C:\temp\newfile.txt", Document.FilePath.Value);
    }
    #endregion
}
