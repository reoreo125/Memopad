using System.Windows.Input;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.ViewModels.Windows;

public class AboutWindowViewModel : BindableBase
{
    [Dependency]
    public ICloseAboutCommand? CloseAboutCommand { get; set; }
}
