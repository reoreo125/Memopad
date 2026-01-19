using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Reoreo125.Memopad.ViewModels.Dialogs;

public class ErrorDialogViewModel : BindableBase, IDialogAware, IDisposable
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    public string? Title { get; set; }
    public string? Message { get; set; }
    public ImageSource? IconSource { get; set; }
    private IntPtr _iconHandle = IntPtr.Zero;

    public DialogCloseListener RequestClose { get; }

    public DelegateCommand OkCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.OK)));

    public ErrorDialogViewModel()
    {
        _iconHandle = SystemIcons.Error.Handle;
        IconSource = Imaging.CreateBitmapSourceFromHIcon(
            _iconHandle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        Title = parameters.GetValue<string>("title");
        Message = parameters.GetValue<string>("message");
    }
    public bool CanCloseDialog() => true;
    public void OnDialogClosed()
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
