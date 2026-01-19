using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IFindNextCommand : ICommand {}

public class FindNextCommand : CommandBase, IFindNextCommand
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

        var result = EditorService.FindNext();

        if(result is false)
        {
            DialogService.ShowInformation($"{Defaults.ApplicationName}", $"\"{EditorService.Document.SearchText.Value}\" が見つかりません。");
        }
    }
}
