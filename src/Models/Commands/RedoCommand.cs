using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IRedoCommand : ICommand {}

public class RedoCommand : CommandBase, IRedoCommand
{
    [Dependency]
    public IEditorService? EditorService { get; set; }

    public override bool CanExecute(object? parameter) => EditorService is not null && EditorService.Document.CanRedo.Value;

    public override void Execute(object? parameter)
    {
        RequireInjected(EditorService);

        if (!CanExecute(null)) return;
        
        EditorService.Redo();
    }
}
