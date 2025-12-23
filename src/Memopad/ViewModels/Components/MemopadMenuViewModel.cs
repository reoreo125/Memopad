using System.Windows.Input;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.ViewModels.Components;

public class MemopadMenuViewModel : BindableBase
{
    [Dependency]
    public IApplicationExitCommand? ApplicationExitCommand { get; set; }
    [Dependency]
    public INewTextFileCommand? NewTextFileCommand { get; set; }
    [Dependency]
    public IOpenTextFileCommand? OpenTextFileCommand { get; set; }
    [Dependency]
    public IOpenAboutCommand? OpenAboutCommand { get; set; }
}
