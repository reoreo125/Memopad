using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.ViewModels.Windows;

public class GoToLineViewModel : BindableBase, IDialogAware
{
    public string? Title => $"行へ移動";
    public DialogCloseListener RequestClose { get; }

    public DelegateCommand CancelCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));

    public BindableReactiveProperty<int> LineIndex { get; }

    public GoToLineViewModel()
    {
        LineIndex = new();
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        LineIndex.Value = parameters.GetValue<int>("lineIndex");
    }
    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
}
