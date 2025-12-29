using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Reoreo125.Memopad.Views.Dialogs
{
    public partial class FontDialog : UserControl
    {
        public FontDialog()
        {
            InitializeComponent();

            SetupPasteHandler(SizeTextBox);
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
