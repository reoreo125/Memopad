using System.Windows;
using System.Windows.Input;
using Reoreo125.Memopad.Views.Windows;

namespace Reoreo125.Memopad.Models.Commands;

public interface IShowAboutWindowCommand : ICommand { }
public class ShowAboutWindowCommand : CommandBase, IShowAboutWindowCommand
{
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        var aboutWindow = new AboutWindowView();
        aboutWindow.Owner = Application.Current.MainWindow;
        aboutWindow.ShowDialog();
    }
}
