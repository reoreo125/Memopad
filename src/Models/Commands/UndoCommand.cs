using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IUndoCommand : ICommand {}

public class UndoCommand : CommandBase, IUndoCommand
{
    [Dependency]
    public IEditorService? EditorService { get; set; }

    public override bool CanExecute(object? parameter) => EditorService is not null && EditorService.Document.CanUndo.Value;

    public override void Execute(object? parameter)
    {
        RequireInjected(EditorService);

        if (!CanExecute(null)) return;
        
        EditorService.Undo();
    }
}
