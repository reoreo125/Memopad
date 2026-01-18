using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.ViewModels.Windows;
using System.Windows;

namespace Reoreo125.Memopad.Tests.Unit.ViewModels.Windows;

public class MainWindowViewModelTests
{
    IEditorService EditorService { get; }
    ISettingsService SettingsService { get; }

    public MainWindowViewModelTests()
    {
        EditorService = Substitute.For<IEditorService>();
        var document = Substitute.For<IEditorDocument>();
        document.Title.Returns(new ReactiveProperty<string>(""));
        document.Text.Returns(new ReactiveProperty<string>(""));
        EditorService.Document.Returns(document);

        SettingsService = Substitute.For<ISettingsService>();
        var settings = Substitute.For<ISettings>();
        settings.FontFamilyName.Returns(new ReactiveProperty<string>(Defaults.FontFamilyName));
        settings.FontStyleName.Returns(new ReactiveProperty<string>(Defaults.GetFontStyleName(Defaults.FontFamilyName)));
        settings.FontSize.Returns(new ReactiveProperty<int>(Defaults.FontSize));
        settings.ZoomLevel.Returns(new ReactiveProperty<int>(Defaults.ZoomLevel));
        settings.IsWordWrap.Returns(new ReactiveProperty<bool>(Defaults.IsWrapping));
        SettingsService.Settings.Returns(settings);
    }

    [Fact(DisplayName = "【正常系】EditorServiceのTitleがViewModelのTitleに反映されること")]
    public void Title_ShouldReflectEditorServiceTitle()
    {
        EditorService.Document.Title.Returns(new ReactiveProperty<string>("Initial Title"));

        var viewModel = new MainWindowViewModel(EditorService, SettingsService);

        Assert.Equal("Initial Title", viewModel.Title.Value);
        
        viewModel.Dispose();
    }

    [Fact(DisplayName = "【正常系】SettingsServiceのIsWordWrapがViewModelのTextWrappingに反映されること")]
    public void TextWrapping_ShouldReflectSettingsServiceIsWordWrap()
    {
        var viewModel = new MainWindowViewModel(EditorService, SettingsService);

        SettingsService.Settings.IsWordWrap.Value = false;
        Assert.Equal(TextWrapping.NoWrap, viewModel.TextWrapping.Value);

        SettingsService.Settings.IsWordWrap.Value = true;
        Assert.Equal(TextWrapping.Wrap, viewModel.TextWrapping.Value);
        
        viewModel.Dispose();
    }

    [Fact(DisplayName = "【正常系】SettingsServiceのFontSize/ZoomLevel変更時にViewModelのFontSizeが更新されること")]
    public void FontSize_ShouldUpdate_OnSettingsChange()
    {
        SettingsService.Settings.FontSize.Returns(new ReactiveProperty<int>(10));
        SettingsService.Settings.ZoomLevel.Returns(new ReactiveProperty<int>(100));

        var viewModel = new MainWindowViewModel(EditorService, SettingsService);

        Assert.Equal(10.0, viewModel.FontSize.Value);

        SettingsService.Settings.FontSize.Value = 20;
        Assert.Equal(20.0, viewModel.FontSize.Value, 5);

        SettingsService.Settings.ZoomLevel.Value = 50;
        Assert.Equal(10.0, viewModel.FontSize.Value, 5);
        
        viewModel.Dispose();
    }

    [Fact(DisplayName = "【正常系】ViewModelのText変更がDebounce後にDocumentのTextに反映されること")]
    public async Task Text_ShouldUpdateDocumentText_AfterDebounce()
    {
        EditorService.Document.Text.Returns(new ReactiveProperty<string>("Initial Content"));

        var viewModel = new MainWindowViewModel(EditorService, SettingsService);

        viewModel.Text.Value = "updated";

        Assert.Equal("Initial Content", EditorService.Document.Text.Value);

        // デバウンス期間（500ms）より長く待つ
        await Task.Delay(Defaults.TextBoxDebounce + 50, TestContext.Current.CancellationToken);

        Assert.Equal("updated", EditorService.Document.Text.Value);
        
        viewModel.Dispose();
    }

    [Fact(DisplayName = "【正常系】SettingsServiceのFontStyleName変更時にViewModelのFontStyle/Weightが更新されること")]
    public void FontStyleAndWeight_ShouldUpdate_OnSettingsChange()
    {
        SettingsService.Settings.FontFamilyName.Returns(new ReactiveProperty<string>(Defaults.FontFamilyName));
        var styleName = Defaults.GetFontStyleName(SettingsService.Settings.FontFamilyName.Value);
        SettingsService.Settings.FontStyleName.Returns(new ReactiveProperty<string>(styleName));

        var viewModel = new MainWindowViewModel(EditorService, SettingsService);

        Assert.Equal(FontStyles.Normal, viewModel.FontStyle.Value);
        Assert.Equal(FontWeights.Normal, viewModel.FontWeight.Value);

        SettingsService.Settings.FontStyleName.Value = "Bold";
        Assert.Equal(FontStyles.Normal, viewModel.FontStyle.Value);
        Assert.Equal(FontWeights.Bold, viewModel.FontWeight.Value);

        SettingsService.Settings.FontStyleName.Value = "Italic";
        Assert.Equal(FontStyles.Italic, viewModel.FontStyle.Value);
        Assert.Equal(FontWeights.Normal, viewModel.FontWeight.Value);

        SettingsService.Settings.FontStyleName.Value = "Bold Italic";
        Assert.Equal(FontStyles.Italic, viewModel.FontStyle.Value);
        Assert.Equal(FontWeights.Bold, viewModel.FontWeight.Value);
        
        viewModel.Dispose();
    }

    [Fact(DisplayName = "【正常系】SettingsServiceのFontFamilyNameがViewModelのFontFamilyに反映されること")]
    public void FontFamily_ShouldReflectSettingsFontFamilyName()
    {
        SettingsService.Settings.FontFamilyName.Returns(new ReactiveProperty<string>("Arial"));

        var viewModel = new MainWindowViewModel(EditorService, SettingsService);

        Assert.Equal("Arial", viewModel.FontFamily.Value);

        SettingsService.Settings.FontFamilyName.Value = "MS UI Gothic";
        Assert.Equal("MS UI Gothic", viewModel.FontFamily.Value);
        
        viewModel.Dispose();
    }
}
