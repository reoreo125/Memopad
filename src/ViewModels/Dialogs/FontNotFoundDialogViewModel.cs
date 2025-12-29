namespace Reoreo125.Memopad.ViewModels.Dialogs
{
    public class FontNotFoundDialogViewModel : BindableBase, IDialogAware
    {
        public string? Title { get; set; }

        public DialogCloseListener RequestClose { get; }

        // 各コマンド
        public DelegateCommand OkCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.OK)));

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = "フォント";
        }
        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
    }
}
