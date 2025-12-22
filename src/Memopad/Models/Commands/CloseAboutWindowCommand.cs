using System.Windows;
using Reoreo125.Memopad.Views.Windows;

namespace Reoreo125.Memopad.Models.Commands;

public class CloseAboutWindowCommand : CommandBase
{
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        Application.Current.Windows.OfType<AboutWindow>().FirstOrDefault()?.Close();
    }
}
