using System.Reflection.Metadata;
using System.Windows.Input;
using R3;

namespace Reoreo125.Memopad.Models.Commands;

public interface IInsertDateTimeCommand : ICommand
{
}

public class InsertDateTimeCommand : CommandBase, IInsertDateTimeCommand
{
    [Dependency]
    public IEditorService? EditorService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if(EditorService is null) throw new Exception(nameof(EditorService));

        var now = DateTime.Now.ToString("H:mm yyyy/MM/dd");
        EditorService.Insert(now);
    }
}
