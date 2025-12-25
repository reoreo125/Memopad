using System.Windows;
using System.Windows.Controls;
using R3;
using Reoreo125.Memopad.Models;

namespace Reoreo125.Memopad.ViewModels.Windows;

public class MainWindowViewModel : BindableBase, IDisposable
{
    public BindableReactiveProperty<string> Title { get; }
    public BindableReactiveProperty<string> Text { get; }
    public BindableReactiveProperty<int> Row { get; }
    public BindableReactiveProperty<int> Column { get; }
    public BindableReactiveProperty<double> FontSize { get; }
    public BindableReactiveProperty<TextWrapping> TextWrapping { get; }
    public BindableReactiveProperty<int> CaretIndex { get; }
    public BindableReactiveProperty<int> SelectionLength { get; }

    protected IEditorService EditorService => _editorService;
    private readonly IEditorService _editorService;
    private DisposableBag _disposableCollection = new();


    public MainWindowViewModel(IEditorService editorService)
    {
        _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));

        #region Model -> ViewModel -> View

        Title = EditorService.Document.Title
            //.Where(_ => EditorService.CanNotification)
            .ToBindableReactiveProperty(string.Empty);

        CaretIndex = new BindableReactiveProperty<int>(0);

        SelectionLength = new BindableReactiveProperty<int>(0);

        Text = new BindableReactiveProperty<string>(string.Empty);
        Text = EditorService.Document.Text
            .Where(value => Text!.Value != value) // 循環防止
            .ToBindableReactiveProperty(string.Empty);

        Row = new BindableReactiveProperty<int>(1);

        Column = new BindableReactiveProperty<int>(1);

        FontSize = EditorService.Settings.ZoomLevel
            .Select(value => Defaults.FontSize * value)
            .ToBindableReactiveProperty(Defaults.FontSize);

        TextWrapping = EditorService.Settings.IsWordWrap
            .Select(value => value ? System.Windows.TextWrapping.Wrap : System.Windows.TextWrapping.NoWrap)
            .ToBindableReactiveProperty(Defaults.TextWrapping);

        CaretIndex = EditorService.Document.CaretIndex
            .ToBindableReactiveProperty(0);

        SelectionLength = EditorService.Document.SelectionLength
            .ToBindableReactiveProperty(0);

        #endregion

        #region View -> ViewModel -> Model
        // TextBoxの内容変更 
        Text.Where(value => value is not null)
            .Debounce(TimeSpan.FromMilliseconds(Defaults.TextBoxDebounce))
            .Subscribe(value => EditorService.Document.Text.Value = value)
            .AddTo(ref _disposableCollection);
        // TextBoxからの行変更
        Row.Where(value => 0 < value)
            .Subscribe(value => EditorService.Document.Row.Value = value)
            .AddTo(ref _disposableCollection);
        // TextBoxからの列変更
        Column.Where(value => 0 < value)
              .Subscribe(value => EditorService.Document.Column.Value = value)
              .AddTo(ref _disposableCollection);
        CaretIndex.Subscribe(value => EditorService.Document.CaretIndex.Value = value)
            .AddTo(ref _disposableCollection);
        SelectionLength.Subscribe(value => EditorService.Document.SelectionLength.Value = value)
            .AddTo(ref _disposableCollection);
        #endregion
    }

    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}


public static class TextBoxBehavior
{
    // --- CaretIndex 用の添付プロパティ ---
    public static readonly DependencyProperty BindableCaretIndexProperty =
        DependencyProperty.RegisterAttached("BindableCaretIndex", typeof(int), typeof(TextBoxBehavior),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCaretIndexChanged));

    public static int GetBindableCaretIndex(DependencyObject obj) => (int)obj.GetValue(BindableCaretIndexProperty);
    public static void SetBindableCaretIndex(DependencyObject obj, int value) => obj.SetValue(BindableCaretIndexProperty, value);

    private static void OnCaretIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox && textBox.CaretIndex != (int)e.NewValue)
        {
            textBox.CaretIndex = (int)e.NewValue;
        }
    }

    // --- SelectionLength 用の添付プロパティ ---
    public static readonly DependencyProperty BindableSelectionLengthProperty =
        DependencyProperty.RegisterAttached("BindableSelectionLength", typeof(int), typeof(TextBoxBehavior),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectionLengthChanged));

    public static int GetBindableSelectionLength(DependencyObject obj) => (int)obj.GetValue(BindableSelectionLengthProperty);
    public static void SetBindableSelectionLength(DependencyObject obj, int value) => obj.SetValue(BindableSelectionLengthProperty, value);

    private static void OnSelectionLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox && textBox.SelectionLength != (int)e.NewValue)
        {
            textBox.SelectionLength = (int)e.NewValue;
        }
    }

    // 初期化時にイベントをフック（View -> ViewModel への同期用）
    public static readonly DependencyProperty ObserveSelectionProperty =
        DependencyProperty.RegisterAttached("ObserveSelection", typeof(bool), typeof(TextBoxBehavior),
            new PropertyMetadata(false, OnObserveSelectionChanged));

    public static bool GetObserveSelection(DependencyObject obj) => (bool)obj.GetValue(ObserveSelectionProperty);
    public static void SetObserveSelection(DependencyObject obj, bool value) => obj.SetValue(ObserveSelectionProperty, value);

    private static void OnObserveSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox)
        {
            if ((bool)e.NewValue)
                textBox.SelectionChanged += TextBox_SelectionChanged;
            else
                textBox.SelectionChanged -= TextBox_SelectionChanged;
        }
    }

    private static void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            SetBindableCaretIndex(textBox, textBox.CaretIndex);
            SetBindableSelectionLength(textBox, textBox.SelectionLength);
        }
    }
}
