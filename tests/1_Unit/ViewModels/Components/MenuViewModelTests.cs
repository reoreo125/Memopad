using System.Windows.Input;
using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using Reoreo125.Memopad.ViewModels.Components;

namespace Reoreo125.Memopad.Tests.Unit.ViewModels.Components;

public class MenuViewModelTests
{
    IEditorService EditorService { get; }
    ISettingsService SettingsService { get; }
    ISettings Settings { get; }

    public MenuViewModelTests()
    {
        EditorService = Substitute.For<IEditorService>();
        SettingsService = Substitute.For<ISettingsService>();
        Settings = Substitute.For<ISettings>();
        Settings.ShowStatusBar.Returns(new ReactiveProperty<bool>(Defaults.ShowStatusBar));
        Settings.IsWordWrap.Returns(new ReactiveProperty<bool>(Defaults.IsWrapping));
        SettingsService.Settings.Returns(Settings);
    }

    #region コンストラクタ
    [Fact(DisplayName = "【異常系】コンストラクタ:EditorServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullEditorService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new MenuViewModel(null!, SettingsService));

        Assert.Equal("editorService", exception.ParamName);
    }

    [Fact(DisplayName = "【異常系】コンストラクタ:SettingsServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullSettingsService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new MenuViewModel(EditorService, null!));

        Assert.Equal("settingsService", exception.ParamName);
    }
    #endregion

    #region 初期値
    [Fact(DisplayName = "【正常系】初期値:設定値が反映されること")]
    public void Constructor_ShouldInitializeFromSettings()
    {
        var viewModel = new MenuViewModel(EditorService, SettingsService);

        Assert.True(viewModel.ShowStatusBar.Value);
        Assert.False(viewModel.IsWordWrap.Value);
    }

    [Fact(DisplayName = "【正常系】初期値:ステータスバー非表示・折り返し有効の設定が反映されること")]
    public void Constructor_WhenSettingsAreNonDefault_ShouldReflectValues()
    {
        Settings.ShowStatusBar.Returns(new ReactiveProperty<bool>(false));
        Settings.IsWordWrap.Returns(new ReactiveProperty<bool>(true));

        var viewModel = new MenuViewModel(EditorService, SettingsService);

        Assert.False(viewModel.ShowStatusBar.Value);
        Assert.True(viewModel.IsWordWrap.Value);
    }
    #endregion

    #region ShowStatusBar
    [Theory(DisplayName = "【正常系】ShowStatusBar:設定値の変更に追従すること")]
    [InlineData(true)]
    [InlineData(false)]
    public void ShowStatusBar_ShouldFollowSettings(bool showStatusBar)
    {
        var viewModel = new MenuViewModel(EditorService, SettingsService);

        Settings.ShowStatusBar.Value = showStatusBar;

        Assert.Equal(showStatusBar, viewModel.ShowStatusBar.Value);
    }
    #endregion

    #region IsWordWrap
    [Theory(DisplayName = "【正常系】IsWordWrap:設定値の変更に追従すること")]
    [InlineData(true)]
    [InlineData(false)]
    public void IsWordWrap_ShouldFollowSettings(bool isWordWrap)
    {
        var viewModel = new MenuViewModel(EditorService, SettingsService);

        Settings.IsWordWrap.Value = isWordWrap;

        Assert.Equal(isWordWrap, viewModel.IsWordWrap.Value);
    }
    #endregion

    #region コマンドプロパティ
    [Fact(DisplayName = "【正常系】コマンドプロパティ:すべてDIで設定可能なICommandであること")]
    public void CommandProperties_ShouldBeWritableAndImplementICommand()
    {
        var commandProperties = typeof(MenuViewModel)
            .GetProperties()
            .Where(property => property.GetCustomAttributes(typeof(DependencyAttribute), inherit: true).Length > 0)
            .ToArray();

        Assert.NotEmpty(commandProperties);
        Assert.All(commandProperties, property =>
        {
            Assert.True(property.CanRead, $"{property.Name} が読み取り可能ではありません。");
            Assert.True(property.CanWrite, $"{property.Name} がDIで設定できません。");
            Assert.True(
                typeof(ICommand).IsAssignableFrom(property.PropertyType),
                $"{property.Name} が ICommand ではありません。");
        });
    }

    [Fact(DisplayName = "【正常系】コマンドプロパティ:設定したコマンドが取得できること")]
    public void CommandProperty_WhenInjected_ShouldReturnSameInstance()
    {
        var command = Substitute.For<INewTextFileCommand>();
        var viewModel = new MenuViewModel(EditorService, SettingsService)
        {
            NewTextFileCommand = command
        };

        Assert.Same(command, viewModel.NewTextFileCommand);
    }
    #endregion
}
