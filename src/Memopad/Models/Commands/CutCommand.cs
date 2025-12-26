using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface ICutCommand : ICommand
{
}

public class CutCommand : CommandBase, ICutCommand
{
    [Dependency]
    public IEditorService? EditorService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (EditorService is null) throw new Exception(nameof(EditorService));

        EditorService.Cut();
    }
}
