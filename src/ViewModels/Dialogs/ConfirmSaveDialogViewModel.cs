using Reoreo125.Memopad.Models;

namespace Reoreo125.Memopad.ViewModels.Dialogs;

public class ConfirmSaveDialogViewModel : BindableBase, IDialogAware
{
    public string? Title => $"{Defaults.ApplicationName}";
    public string? Message { get; set; }

    public DialogCloseListener RequestClose { get; }

    // 各コマンド
    public DelegateCommand SaveCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Yes)));
    public DelegateCommand DontSaveCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.No)));
    public DelegateCommand CancelCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));

    public void OnDialogOpened(IDialogParameters parameters)
    {
        Message = parameters.GetValue<string>("message");
    }
    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
}
