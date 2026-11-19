using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Tests.TestHelpers;
using Reoreo125.Memopad.ViewModels.Dialogs;

namespace Reoreo125.Memopad.Tests.Unit.ViewModels.Dialogs;

public class ReplaceDialogViewModelTests
{
    IEditorService EditorService { get; }
    IEditorDocument Document { get; }
    ISettingsService SettingsService { get; }

    public ReplaceDialogViewModelTests()
    {
        EditorService = Substitute.For<IEditorService>();
        Document = Substitute.For<IEditorDocument>();
        Document.SearchText.Returns(new ReactiveProperty<string>(string.Empty));
        Document.ReplaceText.Returns(new ReactiveProperty<string>(string.Empty));
        Document.SelectedText.Returns(new ReactiveProperty<string>(string.Empty));
        Document.MatchCase.Returns(new ReactiveProperty<bool>(Defaults.MatchCase));
        Document.WrapAround.Returns(new ReactiveProperty<bool>(Defaults.WrapAround));
        EditorService.Document.Returns(Document);

        SettingsService = Substitute.For<ISettingsService>();
    }

    #region コンストラクタ
    [Fact(DisplayName = "【異常系】コンストラクタ:EditorServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullEditorService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new ReplaceDialogViewModel(null!, SettingsService));

        Assert.Equal("editorService", exception.ParamName);
    }

    [Fact(DisplayName = "【異常系】コンストラクタ:SettingsServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullSettingsService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new ReplaceDialogViewModel(EditorService, null!));

        Assert.Equal("settingsService", exception.ParamName);
    }
    #endregion

    #region 初期値
    [Fact(DisplayName = "【正常系】初期値:Documentの検索・置換条件が反映されること")]
    public void Constructor_ShouldReflectDocumentValues()
    {
        Document.SearchText.Returns(new ReactiveProperty<string>("before"));
        Document.ReplaceText.Returns(new ReactiveProperty<string>("after"));
        Document.MatchCase.Returns(new ReactiveProperty<bool>(true));
        Document.WrapAround.Returns(new ReactiveProperty<bool>(true));

        using var viewModel = new ReplaceDialogViewModel(EditorService, SettingsService);

        Assert.Equal("before", viewModel.SearchText.Value);
        Assert.Equal("after", viewModel.ReplaceText.Value);
        Assert.True(viewModel.MatchCase.Value);
        Assert.True(viewModel.WrapAround.Value);
        Assert.True(viewModel.FindNextButton_IsEnabled.Value);
        Assert.True(viewModel.ReplaceNextButton_IsEnabled.Value);
        Assert.True(viewModel.ReplaceAllButton_IsEnabled.Value);
    }

    [Fact(DisplayName = "【正常系】各ボタンの活性:検索文字列が空の場合に無効になること")]
    public void ButtonIsEnabled_ShouldFollowSearchText()
    {
        using var viewModel = new ReplaceDialogViewModel(EditorService, SettingsService);

        Assert.False(viewModel.FindNextButton_IsEnabled.Value);
        Assert.False(viewModel.ReplaceNextButton_IsEnabled.Value);
        Assert.False(viewModel.ReplaceAllButton_IsEnabled.Value);

        viewModel.SearchText.Value = "keyword";
        Assert.True(viewModel.FindNextButton_IsEnabled.Value);
        Assert.True(viewModel.ReplaceNextButton_IsEnabled.Value);
        Assert.True(viewModel.ReplaceAllButton_IsEnabled.Value);

        viewModel.SearchText.Value = string.Empty;
        Assert.False(viewModel.FindNextButton_IsEnabled.Value);
        Assert.False(viewModel.ReplaceNextButton_IsEnabled.Value);
        Assert.False(viewModel.ReplaceAllButton_IsEnabled.Value);
    }
    #endregion

    #region View -> Model
    [Fact(DisplayName = "【正常系】検索・置換条件:ViewModelの変更がDocumentに反映されること")]
    public void ViewModelChanges_ShouldPropagateToDocument()
    {
        using var viewModel = new ReplaceDialogViewModel(EditorService, SettingsService);

        viewModel.SearchText.Value = "before";
        viewModel.ReplaceText.Value = "after";
        viewModel.MatchCase.Value = true;
        viewModel.WrapAround.Value = true;

        Assert.Equal("before", Document.SearchText.Value);
        Assert.Equal("after", Document.ReplaceText.Value);
        Assert.True(Document.MatchCase.Value);
        Assert.True(Document.WrapAround.Value);
    }
    #endregion

    #region OnDialogOpened
    [Fact(DisplayName = "【正常系】OnDialogOpened:選択中の文字列が検索文字列に設定されること")]
    public void OnDialogOpened_ShouldUseSelectedText()
    {
        Document.SelectedText.Returns(new ReactiveProperty<string>("selected"));
        using var viewModel = new ReplaceDialogViewModel(EditorService, SettingsService);

        viewModel.OnDialogOpened(new DialogParameters());

        Assert.Equal("selected", viewModel.SearchText.Value);
        Assert.Equal("selected", Document.SearchText.Value);
    }
    #endregion

    #region CancelCommand
    [Fact(DisplayName = "【正常系】CancelCommand:キャンセルでダイアログが閉じること")]
    public void CancelCommand_ShouldCloseWithCancel()
    {
        using var viewModel = new ReplaceDialogViewModel(EditorService, SettingsService);
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.CancelCommand.Execute();

        Assert.Equal(ButtonResult.Cancel, result?.Result);
    }
    #endregion

    #region Dispose
    [Fact(DisplayName = "【正常系】Dispose:破棄後にViewModelの変更がDocumentに反映されないこと")]
    public void Dispose_ShouldStopViewToModelSubscriptions()
    {
        var viewModel = new ReplaceDialogViewModel(EditorService, SettingsService);
        viewModel.SearchText.Value = "before";
        viewModel.ReplaceText.Value = "before-replace";

        viewModel.Dispose();
        viewModel.SearchText.Value = "after";
        viewModel.ReplaceText.Value = "after-replace";

        Assert.Equal("before", Document.SearchText.Value);
        Assert.Equal("before-replace", Document.ReplaceText.Value);
    }
    #endregion
}
