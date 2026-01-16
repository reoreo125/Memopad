namespace Reoreo125.Memopad.ViewModels.Dialogs
{
    public class FileSaveErrorDialogViewModel : BindableBase, IDialogAware
    {
        public DialogCloseListener RequestClose { get; }

        // 各コマンド
        public DelegateCommand OkCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.OK)));

        public void OnDialogOpened(IDialogParameters parameters){}
        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
    }
}
