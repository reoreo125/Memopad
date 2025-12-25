using System.Text;
using System.Windows;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.ViewModels.Components;

public class MemopadStatusBarViewModel : BindableBase, IDisposable
{
    public BindableReactiveProperty<Visibility> ShowStatusBar { get; }
    public BindableReactiveProperty<string> PositionText { get; }
    public BindableReactiveProperty<string> ZoomLevelText { get; }
    public BindableReactiveProperty<string> LineEndingText { get; }
    public BindableReactiveProperty<string> EncodingText { get; }

    protected IEditorService EditorService => _editorService;
    private readonly IEditorService _editorService;

    private DisposableBag _disposableCollection = new();

    public MemopadStatusBarViewModel(IEditorService editorService)
    {
        _editorService = editorService ?? throw new ArgumentNullException(nameof(EditorService));

        #region Model -> ViewModel -> View

        ShowStatusBar = EditorService.Settings.ShowStatusBar
            .Select(value => value ? Visibility.Visible : Visibility.Collapsed)
            .ToBindableReactiveProperty(Visibility.Visible);
        PositionText = Observable.Merge
            (
                EditorService.Document.Row.Select(_ => string.Empty),
                EditorService.Document.Column.Select(_ => string.Empty)
            )
            .Select(_ => $"{EditorService.Document.Row.Value}行、{EditorService.Document.Column.Value}列")
            .ToBindableReactiveProperty(Defaults.PositionText);
        ZoomLevelText = EditorService.Settings.ZoomLevel
            .Select(value => $"{(int)(value * 100)}%")
            .ToBindableReactiveProperty(Defaults.ZoomLevelText);
        LineEndingText = EditorService.Document.LineEnding
            .Select(value => CreateLineEndingText(value))
            .ToBindableReactiveProperty(CreateLineEndingText(Defaults.LineEnding));
        EncodingText = Observable.Merge
            (
                EditorService.Document.Encoding,
                EditorService.Document.HasBom.Select(_ => EditorService.Document.Encoding.Value)
            )
            .Select(encoding => encoding ?? Encoding.UTF8)
            .Select(encoding =>
            {
                var encodingText = encoding.WebName.ToUpper();
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
            LineEnding.CR => "MacOS (CR)",
            _ => "Unknown"
        };
    }
    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
