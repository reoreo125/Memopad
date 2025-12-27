using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using R3;
using Reoreo125.Memopad.Models.Commands;
using Reoreo125.Memopad.ViewModels.Windows;

namespace Reoreo125.Memopad.Views.Windows;

public partial class MainWindow : Window, IDisposable
{
    [Dependency]
    public ZoomCommand? ZoomCommand { get; set; }
    [Dependency]
    public ExitCommand? ExitCommand { get; set; }

    private DisposableBag _disposableCollection = new();

    public MainWindow()
    {
        InitializeComponent();

        var vm = DataContext as MainWindowViewModel;
        if (vm is null) throw new Exception(nameof(DataContext));

        vm.EditorService.RequestCut
            .Subscribe(_ => EditorBox.Cut())
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestCopy
            .Subscribe(_ => EditorBox.Copy())
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestPaste
            .Subscribe(_ => EditorBox.Paste())
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestDelete
            .Subscribe(_ => EditorBox.SelectedText = "")
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestInsert
            .Subscribe(value =>
            {
                EditorBox.SelectedText = value;

                EditorBox.SelectionLength = 0;
                EditorBox.SelectionStart += value.Length;
            })
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestLoadText.Subscribe(value =>
            {
                EditorBox.IsUndoEnabled = false;
                EditorBox.IsUndoEnabled = true;

                EditorBox.Text = value;

                EditorBox.CaretIndex = 0;
            })
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestReset.Subscribe(_ =>
            {
                EditorBox.IsUndoEnabled = false;
                EditorBox.IsUndoEnabled = true;

                EditorBox.Text = string.Empty;

                EditorBox.ScrollToHome();
                EditorBox.CaretIndex = 0;
                EditorBox.Focus();
            })
            .AddTo (ref _disposableCollection);
        vm.EditorService.RequestUndo
            .Subscribe(_ => EditorBox.Undo())
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestRedo
            .Subscribe(_ => EditorBox.Redo())
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestSelect
            .Subscribe(selection  =>
            {
                var(foundIndex, length) = selection;
                EditorBox.Select(foundIndex, length);
                EditorBox.Focus();
            })
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestSelectAll
            .Subscribe(_ => EditorBox.SelectAll())
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestGoToLine
            .Subscribe(args =>
            {
                int charIndex = EditorBox.GetCharacterIndexFromLineIndex(args.LineIndex);

                if (charIndex != -1)
                {
                    EditorBox.CaretIndex = charIndex;
                    EditorBox.ScrollToLine(args.LineIndex);
                    EditorBox.Focus();

                    args.IsSuccess = true;
                }
            })
            .AddTo(ref _disposableCollection);
    }

    private void EditorBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            var doc = vm.EditorService.Document;

            doc.CanUndo.Value = EditorBox.CanUndo;
            doc.CanRedo.Value = EditorBox.CanRedo;
        }
    }

    void EditorBox_SelectionChanged(object sender, RoutedEventArgs e)
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
    void EditorBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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

    private bool _isclosing = false;
    private async void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        // 既定の動作をキャンセル
        e.Cancel = true;

        // 既に終了処理中なら何もしない（二重動作防止）
        if (_isclosing) return;

        _isclosing = true;
        ExitCommand!.Execute(null);
        _isclosing = false;
    }
    

    public void Dispose()
    {
        _disposableCollection.Dispose();
    }


}
