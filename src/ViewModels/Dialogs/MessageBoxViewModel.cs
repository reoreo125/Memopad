using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Reoreo125.Memopad.ViewModels.Dialogs;

public class InformationDialogViewModel : MessageBoxViewModelBase
{
    public InformationDialogViewModel() : base(SystemIcons.Information) {}
}
public class ErrorDialogViewModel : MessageBoxViewModelBase
{
    public ErrorDialogViewModel() : base(SystemIcons.Error) { }
}
public class WarningDialogViewModel : MessageBoxViewModelBase
{
    public WarningDialogViewModel() : base(SystemIcons.Warning) { }
}

public abstract class MessageBoxViewModelBase : BindableBase, IDialogAware
{
    public string? Title { get => _title; set => SetProperty(ref _title, value); }
    private string? _title;

    public string? Message { get => _message; set => SetProperty(ref _message, value); }
    private string? _message;

    /// <summary>
    /// 表示するアイコン。コンストラクタで確定するため読み取り専用。
    /// </summary>
    public ImageSource? IconSource { get; }

    public DialogCloseListener RequestClose { get; }

    public DelegateCommand OkCommand { get; }

    protected MessageBoxViewModelBase(Icon icon)
    {
        ArgumentNullException.ThrowIfNull(icon);

        // CreateBitmapSourceFromHIcon はアイコンのビットマップを即時にコピーする。
        // 引数のアイコン（SystemIcons の共有インスタンス）のハンドルは所有していないため
        // DestroyIcon での破棄は行わない（破棄すると共有ハンドルが無効化される）。
        IconSource = Imaging.CreateBitmapSourceFromHIcon(
            icon.Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());

        OkCommand = new DelegateCommand(() => RequestClose.Invoke(new DialogResult(ButtonResult.OK)));
    }

    public virtual void OnDialogOpened(IDialogParameters parameters)
    {
        Title = parameters.GetValue<string>("title");
        Message = parameters.GetValue<string>("message");
    }

    public virtual bool CanCloseDialog() => true;

    public virtual void OnDialogClosed() { }
}
