using Reoreo125.Memopad.Models;

namespace Reoreo125.Memopad.ViewModels.Dialogs
{
    public class LineOutOfBoundsDialogViewModel : BindableBase, IDialogAware
    {
        public string? Title => $"{Defaults.ApplicationName} - 行に移動";

        public DialogCloseListener RequestClose { get; }

        // 各コマンド
        public DelegateCommand OkCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.OK)));

        public void OnDialogOpened(IDialogParameters parameters) { }
        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
    }
}
