using System.Windows;
using R3;
using Reoreo125.Memopad.Models.Services;

namespace Reoreo125.Memopad.ViewModels.Windows;

public class MainWindowViewModel : BindableBase, IDisposable
{
    public BindableReactiveProperty<string> Title { get; }
    public BindableReactiveProperty<string> Text { get; }
    public BindableReactiveProperty<int> Row { get; }
    public BindableReactiveProperty<int> Column { get; }
    public BindableReactiveProperty<double> FontSize { get; }
    public BindableReactiveProperty<TextWrapping> TextWrapping { get; }

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
                MemopadCoreService.IsDirty.Select(_ => string.Empty),
                MemopadCoreService.FileName
            )
            .Where(_ => memopadCoreService.CanNotification)
            .Select(_ =>
            {
                var dirtyMark = MemopadCoreService.IsDirty.Value ? "*" : "";
                var fileName = MemopadCoreService.FileNameWithoutExtension.Value;
                return $"{dirtyMark}{fileName} - {MemoPadDefaults.ApplicationName}";
            })
            .ToBindableReactiveProperty(string.Empty);

        // テキストが変更された時
        Text = new BindableReactiveProperty<string>();
        Text = MemopadCoreService.Text
            .Where(_ => MemopadCoreService.CanNotification)
            .Where(value => Text!.Value != value) // 循環防止
            .ToBindableReactiveProperty(string.Empty);
        Row = new BindableReactiveProperty<int>(1);
        Column = new BindableReactiveProperty<int>(1);
        FontSize = MemopadCoreService.ZoomLevel
            .Where(_ => MemopadCoreService.CanNotification)
            .Select(value => MemoPadDefaults.FontSize * value)
            .ToBindableReactiveProperty(MemoPadDefaults.FontSize);
        TextWrapping = MemopadCoreService.IsWordWrap
            .Where(_ => memopadCoreService.CanNotification)
            .Select(value => value ? System.Windows.TextWrapping.Wrap : System.Windows.TextWrapping.NoWrap)
            .ToBindableReactiveProperty(MemoPadDefaults.TextWrapping);
        #endregion

        #region View -> ViewModel -> Model
        // TextBoxの内容変更 
        Text.Where(value => value is not null)
            .Debounce(TimeSpan.FromMilliseconds(MemoPadDefaults.TextBoxDebounce))
            .Subscribe(value => MemopadCoreService.Text.Value = value)
            .AddTo(ref _disposableCollection);
        // TextBoxからの行変更
        Row.Where(value => 0 < value)
            .Subscribe(value => MemopadCoreService.Row.Value = value)
            .AddTo(ref _disposableCollection);
        // TextBoxからの列変更
        Column.Where(value => 0 < value)
              .Subscribe(value => MemopadCoreService.Column.Value = value)
              .AddTo(ref _disposableCollection);
        #endregion
    }

    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
