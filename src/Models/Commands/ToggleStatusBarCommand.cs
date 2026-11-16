using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IToggleStatusBarCommand : ICommand {}

public class ToggleStatusBarCommand : CommandBase, IToggleStatusBarCommand
{
    [Dependency]
    public ISettingsService? SettingsService { get; set; }
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        RequireInjected(SettingsService);

        SettingsService.Settings.ShowStatusBar.Value = !SettingsService.Settings.ShowStatusBar.Value;
    }
}
