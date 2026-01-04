using System.Windows;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.ViewModels.Windows;

public partial class MainWindowViewModel : BindableBase, IDisposable
{
    [Dependency]
    public INewTextFileCommand? NewTextFileCommand { get; set; }
    [Dependency]
    public IOpenNewWindowCommand? OpenNewWindowCommand { get; set; }
    [Dependency]
    public IOpenTextFileCommand? OpenTextFileCommand { get; set; }
    [Dependency]
    public ISaveTextFileCommand? SaveTextFileCommand { get; set; }
    [Dependency]
    public ISaveAsTextFileCommand? SaveAsTextFileCommand { get; set; }
    [Dependency]
    public IOpenPrintCommand? OpenPrintCommand { get; set; }
    [Dependency]
    public IExitCommand? ExitCommand { get; set; }

    [Dependency]
    public ICutCommand? CutCommand { get; set; }
    [Dependency]
    public ICopyCommand? CopyCommand { get; set; }
    [Dependency]
    public IPasteCommand? PasteCommand { get; set; }
    [Dependency]
    public IInsertDateTimeCommand? InsertDateTimeCommand { get; set; }

    [Dependency]
    public IOpenFindCommand? OpenFindCommand { get; set; }
    [Dependency]
    public IFindNextCommand? FindNextCommand { get; set; }
    [Dependency]
    public IFindPrevCommand? FindPrevCommand { get; set; }
    [Dependency]
    public IOpenReplaceCommand? OpenReplaceCommand { get; set; }
    [Dependency]
    public IOpenGoToLineCommand? OpenGoToLineCommand { get; set; }

    [Dependency]
    public IZoomCommand? ZoomCommand { get; set; }

    public BindableReactiveProperty<string> Title { get; }
    public BindableReactiveProperty<string> Text { get; }
    public BindableReactiveProperty<string> FontFamily { get; }
    public BindableReactiveProperty<FontStyle> FontStyle { get; }
    public BindableReactiveProperty<FontWeight> FontWeight { get; }
    public BindableReactiveProperty<double> FontSize { get; }
    public BindableReactiveProperty<TextWrapping> TextWrapping { get; }

    public IEditorService EditorService => _editorService;
    private readonly IEditorService _editorService;
    private ISettingsService SettingsService => _settingsService;
    private readonly ISettingsService _settingsService;

    private DisposableBag _disposableCollection = new();


    public MainWindowViewModel(IEditorService editorService, ISettingsService settingsService)
    {
        _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(SettingsService));

        #region Model -> ViewModel -> View

        Title = EditorService.Document.Title
            .ToBindableReactiveProperty(string.Empty);
        Text = new BindableReactiveProperty<string>(string.Empty);
        FontFamily = SettingsService.Settings.FontFamilyName
            .ToBindableReactiveProperty(Defaults.FontFamilyName);
        FontStyle = SettingsService.Settings.FontStyleName
            .Select(name => FontStyleInfo.FromFontFamily(FontFamily.Value).First(value => value.Name == name).Style)
            .ToBindableReactiveProperty();
        FontWeight = SettingsService.Settings.FontStyleName
            .Select(name => FontStyleInfo.FromFontFamily(FontFamily.Value).First(value => value.Name == name).Weight)
            .ToBindableReactiveProperty();
        FontSize = Observable.Merge(
                SettingsService.Settings.ZoomLevel.AsUnitObservable(),
                SettingsService.Settings.FontSize.AsUnitObservable())
            .Select(value => SettingsService.Settings.FontSize.Value * SettingsService.Settings.ZoomLevel.Value)
            .ToBindableReactiveProperty(Defaults.FontSize);
        TextWrapping = SettingsService.Settings.IsWordWrap
            .Select(value => value ? System.Windows.TextWrapping.Wrap : System.Windows.TextWrapping.NoWrap)
            .ToBindableReactiveProperty(Defaults.TextWrapping);     

        #endregion

        #region View -> ViewModel -> Model
        // TextBoxの内容変更
        Text.Where(value => value is not null)
            .Debounce(TimeSpan.FromMilliseconds(Defaults.TextBoxDebounce))
            .Subscribe(value => EditorService.Document.Text.Value = value)
            .AddTo(ref _disposableCollection);
        #endregion
    }
    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
