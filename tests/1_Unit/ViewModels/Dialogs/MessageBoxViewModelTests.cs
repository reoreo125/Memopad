using Reoreo125.Memopad.Tests.TestHelpers;
using Reoreo125.Memopad.ViewModels.Dialogs;

namespace Reoreo125.Memopad.Tests.Unit.ViewModels.Dialogs;

public class MessageBoxViewModelTests
{
    public static IEnumerable<object[]> MessageBoxViewModelTypes()
    {
        yield return new object[] { nameof(InformationDialogViewModel) };
        yield return new object[] { nameof(ErrorDialogViewModel) };
        yield return new object[] { nameof(WarningDialogViewModel) };
    }

    private static MessageBoxViewModelBase CreateViewModel(string typeName) => typeName switch
    {
        nameof(InformationDialogViewModel) => new InformationDialogViewModel(),
        nameof(ErrorDialogViewModel) => new ErrorDialogViewModel(),
        nameof(WarningDialogViewModel) => new WarningDialogViewModel(),
        _ => throw new ArgumentOutOfRangeException(nameof(typeName))
    };

    [Theory(DisplayName = "【正常系】コンストラクタ:種類に応じたアイコンが設定されること")]
    [MemberData(nameof(MessageBoxViewModelTypes))]
    public void Constructor_ShouldSetIconSource(string typeName)
    {
        var viewModel = CreateViewModel(typeName);

        Assert.NotNull(viewModel.IconSource);
    }

    [Theory(DisplayName = "【正常系】OnDialogOpened:タイトルとメッセージが反映されること")]
    [MemberData(nameof(MessageBoxViewModelTypes))]
    public void OnDialogOpened_ShouldSetTitleAndMessage(string typeName)
    {
        var viewModel = CreateViewModel(typeName);
        var parameters = new DialogParameters
        {
            { "title", "タイトル" },
            { "message", "メッセージ" }
        };

        viewModel.OnDialogOpened(parameters);

        Assert.Equal("タイトル", viewModel.Title);
        Assert.Equal("メッセージ", viewModel.Message);
    }

    [Theory(DisplayName = "【正常系】OkCommand:OKでダイアログが閉じること")]
    [MemberData(nameof(MessageBoxViewModelTypes))]
    public void OkCommand_ShouldCloseWithOk(string typeName)
    {
        var viewModel = CreateViewModel(typeName);
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.OkCommand.Execute();

        Assert.Equal(ButtonResult.OK, result?.Result);
    }

    [Theory(DisplayName = "【正常系】CanCloseDialog:常にtrueを返すこと")]
    [MemberData(nameof(MessageBoxViewModelTypes))]
    public void CanCloseDialog_ShouldReturnTrue(string typeName)
    {
        var viewModel = CreateViewModel(typeName);

        Assert.True(viewModel.CanCloseDialog());
    }
}
