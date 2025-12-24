using System.Text;
using System.Windows;
using R3;
using Reoreo125.Memopad.Models.Services;

namespace Reoreo125.Memopad.ViewModels.Components;

public class MemopadStatusBarViewModel : BindableBase, IDisposable
{
    public BindableReactiveProperty<Visibility> ShowStatusBar { get; }
    public BindableReactiveProperty<string> PositionText { get; }
    public BindableReactiveProperty<string> ZoomLevelText { get; }
    public BindableReactiveProperty<string> LineEndingText { get; }
    public BindableReactiveProperty<string> EncodingText { get; }

    protected IMemopadCoreService MemopadCoreService => _memopadCoreService;
    private readonly IMemopadCoreService _memopadCoreService;

    private DisposableBag _disposableCollection = new();

    public MemopadStatusBarViewModel(IMemopadCoreService memopadCoreService)
    {
        _memopadCoreService = memopadCoreService ?? throw new ArgumentNullException(nameof(memopadCoreService));

        #region Model -> ViewModel -> View

        ShowStatusBar = MemopadCoreService.ShowStatusBar
            .Where(_ => memopadCoreService.CanNotification)
            .Select(value => value ? Visibility.Visible : Visibility.Collapsed)
            .ToBindableReactiveProperty(Visibility.Visible);
        PositionText = Observable.Merge
            (
                MemopadCoreService.Row.Select(_ => string.Empty),
                MemopadCoreService.Column.Select(_ => string.Empty)
            )
            .Where(_ => MemopadCoreService.CanNotification)
            .Select(_ => $"{MemopadCoreService.Row.Value}行、{MemopadCoreService.Column.Value}列")
            .ToBindableReactiveProperty(MemoPadDefaults.PositionText);
        ZoomLevelText = MemopadCoreService.ZoomLevel
            .Where(_ => MemopadCoreService.CanNotification)
            .Select(value => $"{(int)(value * 100)}%")
            .ToBindableReactiveProperty(MemoPadDefaults.ZoomLevelText);
        LineEndingText = MemopadCoreService.LineEnding
            .Where(_ => MemopadCoreService.CanNotification)
            .Select(value => CreateLineEndingText(value))
            .ToBindableReactiveProperty(CreateLineEndingText(MemoPadDefaults.LineEnding));
        EncodingText = MemopadCoreService.Encoding
            .Where(_ => memopadCoreService.CanNotification)
            .Select((encoding) => encoding ?? Encoding.UTF8)
            .Select((encoding) => encoding.WebName.ToUpper())
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
