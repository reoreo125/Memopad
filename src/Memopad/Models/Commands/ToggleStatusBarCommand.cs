using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IToggleStatusBarCommand : ICommand
{
}

public class ToggleStatusBarCommand : CommandBase, IToggleStatusBarCommand
{
    [Dependency]
    public ICoreService? MemopadCoreService { get; set; }
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (MemopadCoreService is null) throw new Exception("MemopadCoreService");

        MemopadCoreService.Settings.ShowStatusBar.Value = !MemopadCoreService.Settings.ShowStatusBar.Value;
    }
}
