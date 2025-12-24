using System.Windows.Input;
using Reoreo125.Memopad.Models.Services;

namespace Reoreo125.Memopad.Models.Commands;

public interface IToggleWordWrapCommand : ICommand
{
}

public class ToggleWordWrapCommand : CommandBase, IToggleWordWrapCommand
{
    [Dependency]
    public IMemopadCoreService? MemopadCoreService { get; set; }
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (MemopadCoreService is null) throw new Exception("MemopadCoreService");

        MemopadCoreService.IsWordWrap.Value = !MemopadCoreService.IsWordWrap.Value;
    }
}
