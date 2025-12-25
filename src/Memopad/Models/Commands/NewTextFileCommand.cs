using System.Windows;
using System.Windows.Input;
using R3;
using Reoreo125.Memopad.Models.History;
using Reoreo125.Memopad.Views.Dialogs;

namespace Reoreo125.Memopad.Models.Commands;

public interface INewTextFileCommand : ICommand { }
public class NewTextFileCommand : CommandBase, INewTextFileCommand
{
    [Dependency]
    public IMemopadCoreService? MemopadCoreService { get; set; }
    [Dependency]
    public IMemopadDialogService? MemopadDialogService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (MemopadCoreService is null) throw new Exception("MemopadCoreService");

        IDialogResult? result = null;

        if (MemopadCoreService.IsDirty.Value)
        {

        }
        

        MemopadCoreService.Initialize();
    }
}
