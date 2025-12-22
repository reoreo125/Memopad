using System.Windows.Input;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.ViewModels.Components;

public class MemopadMenuViewModel
{
    public ICommand ApplicationExitCommand { get; } = new ApplicationExitCommand();
    public ICommand ShowAboutCommand { get; } = new ShowAboutWindowCommand();
}
