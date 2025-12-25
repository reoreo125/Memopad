using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IToggleWordWrapCommand : ICommand
{
}

public class ToggleWordWrapCommand : CommandBase, IToggleWordWrapCommand
{
    [Dependency]
    public ICoreService? MemopadCoreService { get; set; }
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (MemopadCoreService is null) throw new Exception("MemopadCoreService");

        MemopadCoreService.Settings.IsWordWrap.Value = !MemopadCoreService.Settings.IsWordWrap.Value;
    }
}
