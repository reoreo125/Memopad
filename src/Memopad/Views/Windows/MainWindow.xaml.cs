using System.Windows;
using System.Windows.Controls;
using Reoreo125.Memopad.ViewModels.Windows;

namespace Reoreo125.Memopad.Views.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    void OnSelectionChanged(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            // 1. キャレット位置から行インデックス(0開始)を取得
            int lineIndex = textBox.GetLineIndexFromCharacterIndex(textBox.CaretIndex);

            // 2. その行の開始文字インデックスを取得
            int lineStartIndex = textBox.GetCharacterIndexFromLineIndex(lineIndex);

            // 3. 列番号を計算
            int columnIndex = textBox.CaretIndex - lineStartIndex;

            // 4. ViewModelに値を渡す (DataContextをキャスト)
            if (DataContext is MainWindowViewModel vm)
            {
                vm.Row.Value = lineIndex + 1;    // 1開始にする
                vm.Column.Value = columnIndex + 1;
            }
        }
    }
}
