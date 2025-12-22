using R3;

namespace Reoreo125.Memopad.ViewModels.Windows;

public class MainWindowViewModel : IDisposable
{
    public BindableReactiveProperty<string> Title { get; } = new BindableReactiveProperty<string>();
    public string PreviousText { get; set; } = string.Empty;
    public BindableReactiveProperty<string> Text { get; } = new BindableReactiveProperty<string>();
    public BindableReactiveProperty<bool> TextHasChanged { get; } = new BindableReactiveProperty<bool>(false);
    public BindableReactiveProperty<string> FileName { get; } = new BindableReactiveProperty<string>("新規テキスト");

    private DisposableBag _disposableCollection = new();

    public MainWindowViewModel()
    {
        TextHasChanged.Subscribe(_ =>
            {
                Title.Value = $"{FileName.Value} {(TextHasChanged.Value ? "*" : "")}";
            })
            .AddTo(ref _disposableCollection);

        Text.Debounce(TimeSpan.FromMilliseconds(500))
            .Subscribe(text =>
            {
                if (text is null) return;

                if(PreviousText != text)
                {
                    TextHasChanged.Value = true;
                    TextHasChanged.ForceNotify();
                }
            })
            .AddTo(ref _disposableCollection);
    }

    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
