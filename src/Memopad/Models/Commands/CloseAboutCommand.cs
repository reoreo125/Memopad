using System.Windows;
using System.Windows.Input;
using Reoreo125.Memopad.Views.Windows;

namespace Reoreo125.Memopad.Models.Commands;

public interface ICloseAboutCommand : ICommand { }
public class CloseAboutCommand : CommandBase, ICloseAboutCommand
{
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        Application.Current.Windows.OfType<AboutWindowView>().FirstOrDefault()?.Close();
    }
}
