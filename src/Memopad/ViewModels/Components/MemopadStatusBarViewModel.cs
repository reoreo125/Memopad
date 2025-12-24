using System.Text;
using R3;
using Reoreo125.Memopad.Models.Services;

namespace Reoreo125.Memopad.ViewModels.Components;

public class MemopadStatusBarViewModel : BindableBase, IDisposable
{
    public BindableReactiveProperty<string> PositionText { get; } = new BindableReactiveProperty<string>(MemoPadDefaults.PositionText);
    public BindableReactiveProperty<string> LineEndingText { get; } = new BindableReactiveProperty<string>(CreateLineEndingText(MemoPadDefaults.LineEnding));
    public BindableReactiveProperty<string> EncodingText { get; } = new BindableReactiveProperty<string>(MemoPadDefaults.EncodingText);
    protected IMemopadCoreService MemopadCoreService => _memopadCoreService;
    private readonly IMemopadCoreService _memopadCoreService;

    private DisposableBag _disposableCollection = new();

    public MemopadStatusBarViewModel(IMemopadCoreService memopadCoreService)
    {
        _memopadCoreService = memopadCoreService ?? throw new ArgumentNullException(nameof(memopadCoreService));

        #region Model -> ViewModel -> View

        PositionText = Observable.Merge
            (
                MemopadCoreService.Row.Select(_ => string.Empty),
                MemopadCoreService.Column.Select(_ => string.Empty)
            )
            .Where(_ => MemopadCoreService.CanNotification)
            .Select(_ =>
            {
                int row = MemopadCoreService.Row.Value;
                int column = MemopadCoreService.Column.Value;
                return $"{row}行、{column}列";
            })
            .ToBindableReactiveProperty(MemoPadDefaults.PositionText);
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
