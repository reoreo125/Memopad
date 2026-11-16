using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface ISelectAllCommand : ICommand {}

public class SelectAllCommand : CommandBase, ISelectAllCommand
{
    [Dependency]
    public IEditorService? EditorService { get; set; }

    public override bool CanExecute(object? parameter) => EditorService is not null && !string.IsNullOrEmpty(EditorService.Document.Text.Value);

    public override void Execute(object? parameter)
    {
        RequireInjected(EditorService);

        if (!CanExecute(null)) return;
        
        EditorService.SelectAll();
    }
}
