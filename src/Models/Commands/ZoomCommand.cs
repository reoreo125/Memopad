using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IZoomCommand : ICommand { }

public class ZoomCommand : CommandBase, IZoomCommand
{
    [Dependency]
    public ISettingsService? SettingsService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if(SettingsService is null) throw new Exception(nameof(SettingsService));
        if(parameter is null || parameter is not ZoomOperation) return;

        switch((ZoomOperation)parameter)
        {
            case ZoomOperation.In:
                SettingsService.Settings.ZoomLevel.Value = Math.Min(SettingsService.Settings.ZoomLevel.Value + Defaults.ZoomStep, Defaults.ZoomMax);
                break;
            case ZoomOperation.Out:
                SettingsService.Settings.ZoomLevel.Value = Math.Max(SettingsService.Settings.ZoomLevel.Value - Defaults.ZoomStep, Defaults.ZoomMin);
                break;
            case ZoomOperation.Reset:
                SettingsService.Settings.ZoomLevel.Value = Defaults.ZoomLevel;
                break;
            default:
                break;
        }
    }
}
public enum ZoomOperation
{
    Default,
    In,
    Out,
    Reset,
}
