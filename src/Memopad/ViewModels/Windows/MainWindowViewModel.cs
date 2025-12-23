using Prism.Mvvm;
using R3;
using Reoreo125.Memopad.Models.Services;

namespace Reoreo125.Memopad.ViewModels.Windows;

public class MainWindowViewModel : BindableBase, IDisposable
{
    public BindableReactiveProperty<string> Title { get; } = new BindableReactiveProperty<string>();
    public BindableReactiveProperty<string> Text { get; } = new BindableReactiveProperty<string>();
    public BindableReactiveProperty<string> FileName { get; } = new BindableReactiveProperty<string>("新規テキスト");

    private DisposableBag _disposableCollection = new();

    protected IMemopadCoreService MemopadCoreService => _memopadCoreService;
    private readonly IMemopadCoreService _memopadCoreService;

    public MainWindowViewModel(IMemopadCoreService memopadCoreService)
    {
        _memopadCoreService = memopadCoreService ?? throw new ArgumentNullException(nameof(memopadCoreService));

        Title.Value = _memopadCoreService.Title;

        #region View -> ViewModel -> Model
        // テキスト変更 
        Text.Debounce(TimeSpan.FromMilliseconds(0))
            .Subscribe(text =>
            {
                if (text is null) return;

                MemopadCoreService.ChangeText(text);
            })
            .AddTo(ref _disposableCollection);
        #endregion

        #region Model -> ViewModel -> View
        // テキストが変更された時
        MemopadCoreService.TextChanged.Subscribe(text =>
            {
                // テキストボックスがトリガーになっていない場合、つまりモデル側からテキストボックスが更新される場合
                if (Text.Value != text)
                {
                    Text.Value = text;
                    return;
                }
            })
            .AddTo(ref _disposableCollection);
        // 保存されていない変更がある状態になった時
        MemopadCoreService.DirtyChanged.Subscribe(_ =>
            {
                Title.Value = MemopadCoreService.Title;
            })
            .AddTo(ref _disposableCollection);
        #endregion
    }

    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
