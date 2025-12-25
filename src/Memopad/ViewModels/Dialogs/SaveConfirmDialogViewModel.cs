using System;
using System.Collections.Generic;
using System.Text;

namespace Reoreo125.Memopad.ViewModels.Dialogs
{
    public class SaveConfirmDialogViewModel : BindableBase, IDialogAware
    {
        public string? Message { get; private set; }

        public DialogCloseListener RequestClose { get; }

        // 各コマンド
        public DelegateCommand SaveCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Yes)));
        public DelegateCommand DontSaveCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.No)));
        public DelegateCommand CancelCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));

        public void OnDialogOpened(IDialogParameters parameters) => Message = parameters.GetValue<string>("message");
        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
    }
}
