using Reoreo125.Memopad.Models;

namespace Reoreo125.Memopad.ViewModels.Dialogs;

public class NotFoundDialogViewModel : BindableBase, IDialogAware
{
    public string? Title => $"{Defaults.ApplicationName}";
    public string? Message { get; set; }

    public DialogCloseListener RequestClose { get; }

    // 各コマンド
    public DelegateCommand OkCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.OK)));

    public void OnDialogOpened(IDialogParameters parameters)
    {
        Message = parameters.GetValue<string>("message");
    }
    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
}
