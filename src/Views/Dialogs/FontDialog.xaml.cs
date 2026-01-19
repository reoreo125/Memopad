using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using R3;
using Reoreo125.Memopad.ViewModels.Dialogs;

namespace Reoreo125.Memopad.Views.Dialogs;

public partial class FontDialog : UserControl
{
    private DisposableBag _disposableCollection = new();
    public FontDialog()
    {
        InitializeComponent();

        if (DataContext is not FontDialogViewModel vm) throw new Exception(nameof(DataContext));

        vm.FontName
            .Where(name => name != null)
            .Subscribe(name =>
            {
                // 完全一致
                var item = FontListBox.Items.Cast<string>().FirstOrDefault(i => i == name);
                if(item != null)
                {
                    vm.ListBoxFontName.Value = item;
                    FontListBox.ScrollIntoView(item);
                    return;
                }
                // 前方部分一致
                item = vm.FontNames.FirstOrDefault(value => value.StartsWith(name, StringComparison.CurrentCultureIgnoreCase));
                if (item != null)
                {
                    FontListBox.ScrollIntoView(item);
                }
            })
            .AddTo(ref _disposableCollection);
    }
    private void FontListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem != null)
        {
            // 選択された項目まで自動スクロール
            listBox.ScrollIntoView(listBox.SelectedItem);
        }
    }
}
