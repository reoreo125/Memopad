using System.Windows;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.ViewModels.Windows;

public class MainWindowViewModel : BindableBase, IDisposable
{
    [Dependency]
    public INewTextFileCommand? NewTextFileCommand { get; set; }

    [Dependency]
    public ICutCommand? CutCommand { get; set; }
    [Dependency]
    public ICopyCommand? CopyCommand { get; set; }
    [Dependency]
    public IPasteCommand? PasteCommand { get; set; }

    public BindableReactiveProperty<string> Title { get; }
    public BindableReactiveProperty<string> Text { get; }
    public BindableReactiveProperty<string> FontFamily { get; }
    public BindableReactiveProperty<string> FontStyle { get; }
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
            .ToBindableReactiveProperty(Defaults.FontFamilyName);
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
