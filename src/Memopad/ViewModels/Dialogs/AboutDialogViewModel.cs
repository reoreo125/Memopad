using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.ViewModels.Windows;

public class AboutDialogViewModel : BindableBase, IDialogAware
{
    public string? Title => $"{Defaults.ApplicationName} のバージョン情報";
    public string? Message => Defaults.ApplicationName;
    public string VersionText => Defaults.VersionText;
    public DialogCloseListener RequestClose { get; }

    // 各コマンド
    public DelegateCommand OkCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.OK)));

    public void OnDialogOpened(IDialogParameters parameters)
    {
    }
    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
}
