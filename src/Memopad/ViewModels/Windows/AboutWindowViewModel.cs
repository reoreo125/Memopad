using System.Windows.Input;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.ViewModels.Windows;

public class AboutWindowViewModel
{
    public ICommand CloseAboutWindowCommand { get; } = new CloseAboutWindowCommand();
}
