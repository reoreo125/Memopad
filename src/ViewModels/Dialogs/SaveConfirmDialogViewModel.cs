namespace Reoreo125.Memopad.ViewModels.Dialogs;

public class SaveConfirmDialogViewModel : BindableBase, IDialogAware
{
    public string? Title { get; set; }
    public string? Message { get; set; }

    public DialogCloseListener RequestClose { get; }

    // 各コマンド
    public DelegateCommand SaveCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Yes)));
    public DelegateCommand DontSaveCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.No)));
    public DelegateCommand CancelCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));

    public void OnDialogOpened(IDialogParameters parameters)
    {
        Title = parameters.GetValue<string>("title");
        Message = parameters.GetValue<string>("message");
    }
    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
}
