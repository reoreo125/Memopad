using System.Windows;
using System.Windows.Input;
using Reoreo125.Memopad.Models.Services;

namespace Reoreo125.Memopad.Models.Commands;

public interface IZoomCommand : ICommand
{
}

public class ZoomCommand : CommandBase, IZoomCommand
{
    [Dependency]
    public IMemopadCoreService? MemopadCoreService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if(MemopadCoreService is null) throw new Exception("MemopadCoreService");
        if(parameter is null || parameter is not ZoomOperation) return;

        switch((ZoomOperation)parameter)
        {
            case ZoomOperation.In:
                MemopadCoreService.ZoomLevel.Value = Math.Min(MemopadCoreService.ZoomLevel.Value + MemoPadDefaults.ZoomStep, MemoPadDefaults.ZoomMax);
                break;
            case ZoomOperation.Out:
                MemopadCoreService.ZoomLevel.Value = Math.Max(MemopadCoreService.ZoomLevel.Value - MemoPadDefaults.ZoomStep, MemoPadDefaults.ZoomMin);
                break;
            case ZoomOperation.Reset:
                MemopadCoreService.ZoomLevel.Value = MemoPadDefaults.ZoomLevel;
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
