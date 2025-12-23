using System.Windows.Input;
using R3;
using Reoreo125.Memopad.Models.Commands;
using Reoreo125.Memopad.Models.Services;

namespace Reoreo125.Memopad.ViewModels.Components;

public class MemopadStatusBarViewModel : BindableBase, IDisposable
{
    public BindableReactiveProperty<string> PositionText { get; } = new BindableReactiveProperty<string>("1行 1列");

    protected IMemopadCoreService MemopadCoreService => _memopadCoreService;
    private readonly IMemopadCoreService _memopadCoreService;

    private DisposableBag _disposableCollection = new();

    public MemopadStatusBarViewModel(IMemopadCoreService memopadCoreService)
    {
        _memopadCoreService = memopadCoreService ?? throw new ArgumentNullException(nameof(memopadCoreService));

        #region Model -> ViewModel -> View
        MemopadCoreService.SelectionChanged.Subscribe(((int row, int column) pos) =>
            {
                PositionText.Value = $"{pos.row}行 {pos.column}列";
            })
            .AddTo(ref _disposableCollection);
        #endregion
    }
    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
