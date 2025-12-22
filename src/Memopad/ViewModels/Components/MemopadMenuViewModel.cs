using System.Windows.Input;
using Reoreo125.Memopad.Commands;

namespace Reoreo125.Memopad.ViewModels.Components;

public class MemopadMenuViewModel
{
    public ICommand ExitCommand { get; } = new ApplicationExitCommand();
}
