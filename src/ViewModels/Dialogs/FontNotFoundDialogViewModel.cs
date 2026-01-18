using R3;

namespace Reoreo125.Memopad.ViewModels.Dialogs;

public class FontNotFoundDialogViewModel : BindableBase, IDialogAware
{
    public string? Title => "フォント";
    public BindableReactiveProperty<string> Message { get; } = new();
    public DialogCloseListener RequestClose { get; }

    public DelegateCommand OkCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.OK)));

    public void OnDialogOpened(IDialogParameters parameters)
    {
        Message.Value = parameters.GetValue<string>("message");
    }
    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
}
