using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Reoreo125.Memopad.Models.Commands;

public interface IOpenFindCommand : ICommand {}

public class OpenFindCommand : CommandBase, IOpenFindCommand
{
    [Dependency]
    public IDialogService? DialogService { get; set; }
    [Dependency]
    public IEditorService? EditorService { get; set; }
    [Dependency]
    public ISettingsService? SettingsService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (DialogService is null) throw new Exception(nameof(DialogService));
        if (EditorService is null) throw new Exception(nameof(EditorService));
        if (SettingsService is null) throw new Exception(nameof(SettingsService));

        var result = DialogService.ShowFind();
    }
}
