using System.Windows.Input;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.ViewModels.Components;

public class MemopadMenuViewModel : BindableBase
{
    [Dependency]
    public IApplicationExitCommand? ApplicationExitCommand { get; set; }
    [Dependency]
    public IOpenAboutWindowCommand? ShowAboutCommand { get; set; }
}
