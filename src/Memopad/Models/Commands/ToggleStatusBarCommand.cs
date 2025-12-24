using System.Windows.Input;
using Reoreo125.Memopad.Models.Services;

namespace Reoreo125.Memopad.Models.Commands;

public interface IToggleStatusBarCommand : ICommand
{
}

public class ToggleStatusBarCommand : CommandBase, IToggleStatusBarCommand
{
    [Dependency]
    public IMemopadCoreService? MemopadCoreService { get; set; }
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (MemopadCoreService is null) throw new Exception("MemopadCoreService");

        MemopadCoreService.ShowStatusBar.Value = !MemopadCoreService.ShowStatusBar.Value;
    }
}
