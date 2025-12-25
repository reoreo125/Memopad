using System.Windows.Input;
using R3;

namespace Reoreo125.Memopad.Models.Commands;

public interface IInsertDateTimeCommand : ICommand
{
}

public class InsertDateTimeCommand : CommandBase, IInsertDateTimeCommand
{
    [Dependency]
    public ICoreService? MemopadCoreService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if(MemopadCoreService is null) throw new Exception("MemopadCoreService");

        MemopadCoreService.InsertDateTime.OnNext(DateTime.Now);
    }
}
