using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using Reoreo125.Memopad.ViewModels.Windows;

namespace Reoreo125.Memopad.Views.Windows;

public partial class MainWindow : Window, IDisposable
{
    [Dependency]
    public ZoomCommand? ZoomCommand { get; set; }

    private DisposableBag _disposableCollection = new();

    public MainWindow()
    {
        InitializeComponent();

        var vm = DataContext as MainWindowViewModel;
        if (vm is null) throw new Exception(nameof(DataContext));

        vm.EditorService.RequestCut
            .Subscribe(_ =>
            {
                EditorBox.Cut();
            })
            .AddTo(ref _disposableCollection);

    }
    void OnSelectionChanged(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            // 現在のカーソル位置（0から始まるインデックス）
            int caretIndex = textBox.CaretIndex;

            // 0文字目からカーソル位置までのテキストを切り出す
            string textUntilCaret = textBox.Text.Substring(0, caretIndex);

            // 行数：そこまでに含まれる改行コード '\n' の数 + 1
            int line = textUntilCaret.Count(c => c == '\n') + 1;

            // 列数：最後の改行コードのインデックスを探し、そこからの距離を測る
            int lastNewLineIndex = textUntilCaret.LastIndexOf('\n');
            int column = caretIndex - lastNewLineIndex;

            if (DataContext is MainWindowViewModel vm)
            {
                vm.Row.Value = line;
                vm.Column.Value = column;
            }
        }
    }
    void TextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // Ctrlキーが押されているかチェック
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            switch (e.Delta)
            {
                case > 0:
                    ZoomCommand?.Execute(ZoomOperation.In);
                    break;
                case < 0:
                    ZoomCommand?.Execute(ZoomOperation.Out);
                    break;
                case 0:
                    ZoomCommand?.Execute(ZoomOperation.Default);
                    break;
                default:
            }
            // イベントをここで完結させ、通常のスクロールを防ぐ
            e.Handled = true;
        }
    }

    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
