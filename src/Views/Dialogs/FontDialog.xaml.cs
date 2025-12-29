using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.ViewModels.Dialogs;
using Reoreo125.Memopad.ViewModels.Windows;

namespace Reoreo125.Memopad.Views.Dialogs
{
    public partial class FontDialog : UserControl
    {
        private DisposableBag _disposableCollection = new();
        public FontDialog()
        {
            InitializeComponent();

            SetupPasteHandler(SizeTextBox);

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
        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 正規表現で数字(0-9)以外を弾く
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void NumberOnly_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // スペースキーを禁止
            if (e.Key == Key.Space) e.Handled = true;
        }
        // 貼り付け（Ctrl+V）対策：コンストラクタなどで追加
        private void SetupPasteHandler(TextBox textBox)
        {
            DataObject.AddPastingHandler(textBox, (s, e) =>
            {
                if (e.DataObject.GetDataPresent(typeof(string)))
                {
                    var text = (string)e.DataObject.GetData(typeof(string));
                    // 数字以外が含まれる文字列の貼り付けをキャンセル
                    if (!System.Text.RegularExpressions.Regex.IsMatch(text, "^[0-9]+$"))
                    {
                        e.CancelCommand();
                    }
                }
                else { e.CancelCommand(); }
            });
        }
    }
}
