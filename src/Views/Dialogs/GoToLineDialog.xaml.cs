using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Reoreo125.Memopad.Views.Dialogs;

public partial class GoToLineDialog : UserControl
{
    public GoToLineDialog()
    {
        InitializeComponent();

        // 貼り付け（Ctrl+V）を制限
        DataObject.AddPastingHandler(LineIndexTextBox, (s, e) =>
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (!System.Text.RegularExpressions.Regex.IsMatch(text, "^[0-9]+$"))
                {
                    e.CancelCommand();
                }
            }
            else { e.CancelCommand(); }
        });
    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[0-9]+$");
    }

    private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space) e.Handled = true;
    }
}
