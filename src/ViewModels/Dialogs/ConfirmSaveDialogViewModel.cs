using Reoreo125.Memopad.Models;

namespace Reoreo125.Memopad.ViewModels.Dialogs;

public class ConfirmSaveDialogViewModel : BindableBase, IDialogAware
{
    public string? Title => $"{Defaults.ApplicationName}";
    public string? Message { get; set; }

    public DialogCloseListener RequestClose { get; }

    public DelegateCommand SaveCommand { get; }
    public DelegateCommand DontSaveCommand { get; }
    public DelegateCommand CancelCommand { get; }

    public ConfirmSaveDialogViewModel()
    {
        SaveCommand = new DelegateCommand(() => RequestClose.Invoke(new DialogResult(ButtonResult.Yes)));
        DontSaveCommand = new DelegateCommand(() => RequestClose.Invoke(new DialogResult(ButtonResult.No)));
        CancelCommand = new DelegateCommand(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        Message = parameters.GetValue<string>("message");
    }
    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
}
