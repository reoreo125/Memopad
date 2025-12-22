using System.Windows;
using Memopad.Commands;
using Reoreo125.Memopad.Views.Windows;

namespace Reoreo125.Memopad.Commands;

public class ShowAboutWindowCommand : CommandBase
{
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        var aboutWindow = new AboutWindow();
        aboutWindow.Owner = Application.Current.MainWindow;
        aboutWindow.ShowDialog();
    }
}
