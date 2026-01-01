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

        if (DataContext is not MainWindowViewModel vm) throw new Exception(nameof(DataContext));

        vm.EditorService.RequestCut
            .Subscribe(_ => EditorBox.Cut())
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestCopy
            .Subscribe(_ => EditorBox.Copy())
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestPaste
            .Subscribe(_ =>
            {
                EditorBox.Paste();
                EditorBox.Focus();
            })
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

                EditorBox.Focus();
            })
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestLoadText.Subscribe(value =>
            {
                EditorBox.IsUndoEnabled = false;
                EditorBox.IsUndoEnabled = true;

                EditorBox.Text = value;

                EditorBox.CaretIndex = 0;
                EditorBox.Focus();
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
        vm.EditorService.RequestFind
            .Subscribe(args =>
            {
                if (string.IsNullOrEmpty(args.SearchText))
                {
                    args.IsSuccess = false;
                    return;
                }

                string text = EditorBox.Text;
                StringComparison options = args.MatchCase
                    ? StringComparison.CurrentCulture
                    : StringComparison.CurrentCultureIgnoreCase;

                int caret = EditorBox.CaretIndex;
                int selLen = EditorBox.SelectionLength;
                int foundIndex = -1;

                if (args.SearchUp)
                {
                    int startIndex = caret - 1;

                    if (startIndex >= 0)
                    {
                        foundIndex = text.LastIndexOf(args.SearchText, startIndex, options);
                    }

                    if (foundIndex == -1 && args.WrapAround && text.Length > 0)
                    {
                        foundIndex = text.LastIndexOf(args.SearchText, text.Length - 1, options);
                    }
                }
                else
                {
                    int startIndex = Math.Min(text.Length, caret + selLen);

                    foundIndex = text.IndexOf(args.SearchText, startIndex, options);

                    if (foundIndex == -1 && args.WrapAround)
                    {
                        foundIndex = text.IndexOf(args.SearchText, 0, options);
                    }
                }

                if (foundIndex != -1)
                {
                    EditorBox.Select(foundIndex, args.SearchText.Length);

                    int lineIndex = EditorBox.GetLineIndexFromCharacterIndex(foundIndex);
                    EditorBox.ScrollToLine(lineIndex);

                    EditorBox.Focus();
                    args.IsSuccess = true;
                }
                else
                {
                    args.IsSuccess = false;
                }
            })
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
                int targetIndex = args.LineIndex - 1;

                if (targetIndex < 0 || targetIndex >= EditorBox.LineCount)
                {
                    args.IsSuccess = false;
                    return;
                }

                int charIndex = EditorBox.GetCharacterIndexFromLineIndex(targetIndex);

                if (charIndex != -1)
                {
                    EditorBox.CaretIndex = charIndex;
                    EditorBox.ScrollToLine(targetIndex);
                    EditorBox.Focus();

                    args.IsSuccess = true;
                }
            })
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestReplaceNext
            .Subscribe(args =>
            {
                string text = EditorBox.Text;

                int startIndex = EditorBox.CaretIndex;

                StringComparison comparison = args.MatchCase
                    ? StringComparison.CurrentCulture
                    : StringComparison.CurrentCultureIgnoreCase;

                int foundIndex = text.IndexOf(args.SearchText, startIndex, comparison);

                if (foundIndex == -1 && args.WrapAround)
                {
                    foundIndex = text.IndexOf(args.SearchText, 0, comparison);
                }

                if (foundIndex != -1)
                {
                    EditorBox.Select(foundIndex, args.SearchText.Length);
                    EditorBox.SelectedText = args.ReplaceText;

                    int targetLineIndex = EditorBox.GetLineIndexFromCharacterIndex(foundIndex);
                    EditorBox.ScrollToLine(targetLineIndex);
                    EditorBox.CaretIndex = foundIndex + args.ReplaceText.Length;
                    EditorBox.Focus();

                    args.IsSuccess = true;
                }
                else
                {
                    args.IsSuccess = false;
                }
            })
            .AddTo(ref _disposableCollection);
        vm.EditorService.RequestReplaceAll
            .Subscribe(args =>
            {

                if (string.IsNullOrEmpty(args.SearchText))
                {
                    args.IsSuccess = false;
                    return;
                }

                string currentText = EditorBox.Text;
                var options = args.MatchCase ? System.Text.RegularExpressions.RegexOptions.None
                                             : System.Text.RegularExpressions.RegexOptions.IgnoreCase;

                var regex = new System.Text.RegularExpressions.Regex(
                    System.Text.RegularExpressions.Regex.Escape(args.SearchText), options);

                var matches = regex.Matches(currentText);
                var replacedCount = matches.Count;

                if (replacedCount == 0)
                {
                    args.IsSuccess = false;
                    return;
                }

                string newText = regex.Replace(currentText, args.ReplaceText);

                EditorBox.BeginChange();
                try
                {
                    EditorBox.SelectAll();
                    EditorBox.SelectedText = newText;

                    EditorBox.CaretIndex = 0;
                    EditorBox.ScrollToHome();
                    EditorBox.Focus();
                }
                finally
                {
                    EditorBox.EndChange();
                }

                args.IsSuccess = true;
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
            int caretIndex = textBox.CaretIndex;

            string textUntilCaret = textBox.Text.Substring(0, caretIndex);

            int line = textUntilCaret.Count(c => c == '\n') + 1;

            int lastNewLineIndex = textUntilCaret.LastIndexOf('\n');
            int column = caretIndex - lastNewLineIndex;

            if (DataContext is MainWindowViewModel vm)
            {
                vm.EditorService.Document.Row.Value = line;
                vm.EditorService.Document.Column.Value = column;

                vm.EditorService.Document.CaretIndex.Value = textBox.CaretIndex;
                vm.EditorService.Document.SelectedText.Value = textBox.SelectedText;
                vm.EditorService.Document.SelectionLength.Value = textBox.SelectionLength;
            }
        }
    }
    void EditorBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
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
