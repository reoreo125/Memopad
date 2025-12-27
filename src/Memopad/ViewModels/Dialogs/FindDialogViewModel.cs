namespace Reoreo125.Memopad.ViewModels.Dialogs
{
    public class FindDialogViewModel : BindableBase, IDialogAware
    {
        public string? Title => "検索";

        public DialogCloseListener RequestClose { get; }

        // 各コマンド
        public DelegateCommand SaveCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Yes)));
        public DelegateCommand DontSaveCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.No)));
        public DelegateCommand CancelCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
    }
}
