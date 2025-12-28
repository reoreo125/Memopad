using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IFindPrevCommand : ICommand {}

public class FindPrevCommand : CommandBase, IFindPrevCommand
{
    [Dependency]
    public IDialogService? DialogService { get; set; }
    [Dependency]
    public IEditorService? EditorService { get; set; }

    public override bool CanExecute(object? parameter) => !string.IsNullOrEmpty(EditorService!.Document.SearchText.Value);

    public override void Execute(object? parameter)
    {
        if (DialogService is null) throw new Exception(nameof(DialogService));
        if (EditorService is null) throw new Exception(nameof(EditorService));

        var result = EditorService.FindPrev();

        if(result is false)
        {
            DialogService.ShowNotFound(EditorService.Document.SearchText.Value);
        }
    }
}
