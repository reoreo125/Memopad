using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IToggleWordWrapCommand : ICommand {}

public class ToggleWordWrapCommand : CommandBase, IToggleWordWrapCommand
{
    [Dependency]
    public ISettingsService? SettingsService { get; set; }
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (SettingsService is null) throw new Exception(nameof(SettingsService));

        SettingsService.Settings.IsWordWrap.Value = !SettingsService.Settings.IsWordWrap.Value;
    }
}
