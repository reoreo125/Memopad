using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface ICopyCommand : ICommand {}

public class CopyCommand : CommandBase, ICopyCommand
{
    [Dependency]
    public IEditorService? EditorService { get; set; }

    public override bool CanExecute(object? parameter) => EditorService?.Document.SelectionLength.Value > 0;

    public override void Execute(object? parameter)
    {
        if (EditorService is null) throw new Exception(nameof(EditorService));

        if (!CanExecute(null)) return;
        
        EditorService.Copy();
    }
}
