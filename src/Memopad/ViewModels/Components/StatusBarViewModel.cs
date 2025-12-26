using System.Text;
using System.Windows;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.ViewModels.Components;

public class StatusBarViewModel : BindableBase, IDisposable
{
    public BindableReactiveProperty<Visibility> ShowStatusBar { get; }
    public BindableReactiveProperty<string> PositionText { get; }
    public BindableReactiveProperty<string> ZoomLevelText { get; }
    public BindableReactiveProperty<string> LineEndingText { get; }
    public BindableReactiveProperty<string> EncodingText { get; }

    protected IEditorService EditorService => _editorService;
    private readonly IEditorService _editorService;
    protected ISettingsService SettingsService => _settingsService;
    private readonly ISettingsService _settingsService;


    private DisposableBag _disposableCollection = new();

    public StatusBarViewModel(IEditorService editorService, ISettingsService settingsService)
    {
        _editorService = editorService ?? throw new ArgumentNullException(nameof(EditorService));
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(SettingsService));

        #region Model -> ViewModel -> View

        ShowStatusBar = SettingsService.Settings.ShowStatusBar
            .Select(value => value ? Visibility.Visible : Visibility.Collapsed)
            .ToBindableReactiveProperty(Visibility.Visible);
        PositionText = Observable.Merge
            (
                EditorService.Document.Row.AsUnitObservable(),
                EditorService.Document.Column.AsUnitObservable()
            )
            .Select(_ => $"{EditorService.Document.Row.Value}行、{EditorService.Document.Column.Value}列")
            .ToBindableReactiveProperty(Defaults.PositionText);
        ZoomLevelText = SettingsService.Settings.ZoomLevel
            .Select(value => $"{(int)(value * 100)}%")
            .ToBindableReactiveProperty(Defaults.ZoomLevelText);
        LineEndingText = EditorService.Document.LineEnding
            .Select(value => CreateLineEndingText(value))
            .ToBindableReactiveProperty(CreateLineEndingText(Defaults.LineEnding));
        EncodingText = Observable.Merge
            (
                EditorService.Document.Encoding.AsUnitObservable(),
                EditorService.Document.HasBom.AsUnitObservable()
            )
            .Select(_ =>
            {
                var encodingText = EditorService.Document.Encoding.Value.WebName.ToUpper();
                var bomText = EditorService.Document.HasBom.Value ? "(BOM 付き)" : string.Empty;
                return $"{encodingText} {bomText}";
            })
            .ToBindableReactiveProperty(string.Empty);

        #endregion
    }
    static string CreateLineEndingText(LineEnding lineEnding)
    {
        return lineEnding switch
        {
            LineEnding.CRLF => "Windows (CRLF)",
            LineEnding.LF => "Linux (LF)",
            LineEnding.CR => "Macintosh (CR)",
            _ => "Unknown"
        };
    }
    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
