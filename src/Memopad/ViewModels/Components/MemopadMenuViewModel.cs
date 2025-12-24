using System.Windows;
using System.Windows.Input;
using R3;
using Reoreo125.Memopad.Models.Commands;
using Reoreo125.Memopad.Models.Services;

namespace Reoreo125.Memopad.ViewModels.Components;

public class MemopadMenuViewModel : BindableBase
{
    [Dependency]
    public IApplicationExitCommand? ApplicationExitCommand { get; set; }
    [Dependency]
    public INewTextFileCommand? NewTextFileCommand { get; set; }
    [Dependency]
    public IOpenTextFileCommand? OpenTextFileCommand { get; set; }
    [Dependency]
    public IZoomCommand? ZoomCommand { get; set; }
    [Dependency]
    public IOpenAboutCommand? OpenAboutCommand { get; set; }
    [Dependency]
    public IToggleStatusBarCommand? ToggleStatusBarCommand { get; set; }

    public BindableReactiveProperty<bool> ShowStatusBar { get; }

    protected IMemopadCoreService MemopadCoreService => _memopadCoreService;
    private readonly IMemopadCoreService _memopadCoreService;

    public MemopadMenuViewModel(IMemopadCoreService memopadCoreService)
    {
        _memopadCoreService = memopadCoreService ?? throw new ArgumentNullException(nameof(memopadCoreService));

        ShowStatusBar = MemopadCoreService.ShowStatusBar
            .Where(_ => memopadCoreService.CanNotification)
            .ToBindableReactiveProperty(true);
    }
}
