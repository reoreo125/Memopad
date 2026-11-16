using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IReplaceAllCommand : ICommand {}

public class ReplaceAllCommand : CommandBase, IReplaceAllCommand
{
    [Dependency]
    public IDialogService? DialogService { get; set; }
    [Dependency]
    public IEditorService? EditorService { get; set; }

    public override bool CanExecute(object? parameter) => EditorService is not null && !string.IsNullOrEmpty(EditorService.Document.SearchText.Value);

    public override void Execute(object? parameter)
    {
        RequireInjected(DialogService);
        RequireInjected(EditorService);

        var result = EditorService.ReplaceAll();

        if(result is false)
        {
            DialogService.ShowInformation($"{Defaults.ApplicationName}", $"\"{EditorService.Document.SearchText.Value}\" が見つかりません。");
        }
    }
}
