using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IZoomCommand : ICommand
{
}

public class ZoomCommand : CommandBase, IZoomCommand
{
    [Dependency]
    public IEditorService? EditorService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if(EditorService is null) throw new Exception(nameof(EditorService));
        if(parameter is null || parameter is not ZoomOperation) return;

        switch((ZoomOperation)parameter)
        {
            case ZoomOperation.In:
                EditorService.Settings.ZoomLevel.Value = Math.Min(EditorService.Settings.ZoomLevel.Value + Defaults.ZoomStep, Defaults.ZoomMax);
                break;
            case ZoomOperation.Out:
                EditorService.Settings.ZoomLevel.Value = Math.Max(EditorService.Settings.ZoomLevel.Value - Defaults.ZoomStep, Defaults.ZoomMin);
                break;
            case ZoomOperation.Reset:
                EditorService.Settings.ZoomLevel.Value = Defaults.ZoomLevel;
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
