using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Reoreo125.Memopad.Views.Behaviors;

public class NumericInputBehavior : Behavior<TextBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewTextInput += OnPreviewTextInput;
        AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        DataObject.AddPastingHandler(AssociatedObject, OnPasting);
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
        AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
        DataObject.RemovePastingHandler(AssociatedObject, OnPasting);
    }

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space) e.Handled = true;
    }

    private void OnPasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(DataFormats.Text))
        {
            string text = (string)e.DataObject.GetData(DataFormats.Text);
            if (!Regex.IsMatch(text, "^[0-9]+$")) e.CancelCommand();
        }
        else
        {
            e.CancelCommand();
        }
    }
}
