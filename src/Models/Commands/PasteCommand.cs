using System.Windows;
using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IPasteCommand : ICommand {}

public class PasteCommand : CommandBase, IPasteCommand
{
    [Dependency]
    public IEditorService? EditorService { get; set; }

    public override bool CanExecute(object? parameter) => Clipboard.ContainsText(); 

    public override void Execute(object? parameter)
    {
        if (EditorService is null) throw new Exception(nameof(EditorService));

        if (!CanExecute(null)) return;
        
        EditorService.Paste();
    }
}
