namespace Reoreo125.Memopad.ViewModels.Dialogs;

public class FileLoadErrorDialogViewModel : BindableBase, IDialogAware
{
    public DialogCloseListener RequestClose { get; }

    public DelegateCommand OkCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.OK)));

    public void OnDialogOpened(IDialogParameters parameters){}
    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
}
