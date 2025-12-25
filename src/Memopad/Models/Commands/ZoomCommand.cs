using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IZoomCommand : ICommand
{
}

public class ZoomCommand : CommandBase, IZoomCommand
{
    [Dependency]
    public ICoreService? MemopadCoreService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if(MemopadCoreService is null) throw new Exception("MemopadCoreService");
        if(parameter is null || parameter is not ZoomOperation) return;

        switch((ZoomOperation)parameter)
        {
            case ZoomOperation.In:
                MemopadCoreService.Settings.ZoomLevel.Value = Math.Min(MemopadCoreService.Settings.ZoomLevel.Value + Defaults.ZoomStep, Defaults.ZoomMax);
                break;
            case ZoomOperation.Out:
                MemopadCoreService.Settings.ZoomLevel.Value = Math.Max(MemopadCoreService.Settings.ZoomLevel.Value - Defaults.ZoomStep, Defaults.ZoomMin);
                break;
            case ZoomOperation.Reset:
                MemopadCoreService.Settings.ZoomLevel.Value = Defaults.ZoomLevel;
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
