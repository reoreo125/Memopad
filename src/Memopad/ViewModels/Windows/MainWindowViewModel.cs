using System.Data.Common;
using System.Windows;
using Prism.Mvvm;
using R3;
using Reoreo125.Memopad.Models.Services;

namespace Reoreo125.Memopad.ViewModels.Windows;

public class MainWindowViewModel : BindableBase, IDisposable
{
    public BindableReactiveProperty<string> Title { get; }
    public BindableReactiveProperty<string> Text { get; }
    public BindableReactiveProperty<int> Row { get; }
    public BindableReactiveProperty<int> Column { get; }

    protected IMemopadCoreService MemopadCoreService => _memopadCoreService;
    private readonly IMemopadCoreService _memopadCoreService;
    private DisposableBag _disposableCollection = new();

    public MainWindowViewModel(IMemopadCoreService memopadCoreService)
    {
        _memopadCoreService = memopadCoreService ?? throw new ArgumentNullException(nameof(memopadCoreService));

        #region Model -> ViewModel -> View
        // 保存されていない変更がある状態になった時（＊）やタイトル変更
        Title = Observable.Merge
            (
                MemopadCoreService.DirtyChanged.Select(_ => MemopadCoreService.Title),
                MemopadCoreService.TitleChanged
            )
            .ToBindableReactiveProperty(MemopadCoreService.Title);
        // テキストが変更された時
        Text = MemopadCoreService.TextChanged
            .Where(value => Text!.Value != value) // 循環防止
            .ToBindableReactiveProperty(string.Empty);
        Row = new BindableReactiveProperty<int>(1);
        Column = new BindableReactiveProperty<int>(1);
        #endregion

        #region View -> ViewModel -> Model
        // テキスト変更 
        Text.Where(value => value is not null)
            .Debounce(TimeSpan.FromMilliseconds(500))
            .Subscribe(value => MemopadCoreService.ChangeText(value))
            .AddTo(ref _disposableCollection);
        Row.Where(value => 0 < value)
            .Subscribe(value => MemopadCoreService.ChangeSelection(value, MemopadCoreService.Column))
            .AddTo(ref _disposableCollection);
        Column.Where(value => 0 < value)
              .Subscribe(value => MemopadCoreService.ChangeSelection(MemopadCoreService.Row, value))
              .AddTo(ref _disposableCollection);
        #endregion
    }

    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
