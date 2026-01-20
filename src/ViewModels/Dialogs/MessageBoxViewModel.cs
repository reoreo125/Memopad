using System.Drawing;
using System.Runtime.InteropServices;
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

public abstract class MessageBoxViewModelBase : BindableBase, IDialogAware, IDisposable
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    public string? Title { get; set; }
    public string? Message { get; set; }
    public ImageSource? IconSource { get; set; }
    private IntPtr _iconHandle = IntPtr.Zero;

    public DialogCloseListener RequestClose { get; }

    public DelegateCommand OkCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.OK)));

    protected MessageBoxViewModelBase(Icon icon)
    {
        if (icon == null) throw new ArgumentNullException(nameof(icon));
        _iconHandle = icon.Handle;
        IconSource = Imaging.CreateBitmapSourceFromHIcon(
            _iconHandle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
    }

    public virtual void OnDialogOpened(IDialogParameters parameters)
    {
        Title = parameters.GetValue<string>("title");
        Message = parameters.GetValue<string>("message");
    }

    public virtual bool CanCloseDialog() => true;

    public virtual void OnDialogClosed()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (_iconHandle != IntPtr.Zero)
        {
            DestroyIcon(_iconHandle);
            _iconHandle = IntPtr.Zero;
        }
        GC.SuppressFinalize(this);
    }
}
