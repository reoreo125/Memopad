using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;

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
    [Dependency]
    public IToggleWordWrapCommand? ToggleWordWrapCommand { get; set; }
    [Dependency]
    public IInsertDateTimeCommand? InsertDateTimeCommand { get; set; }
    [Dependency]
    public ISaveAsTextFileCommand? SaveAsTextFileCommand { get; set; }

    public BindableReactiveProperty<bool> ShowStatusBar { get; }
    public BindableReactiveProperty<bool> IsWordWrap { get; }

    protected IMemopadCoreService MemopadCoreService => _memopadCoreService;
    private readonly IMemopadCoreService _memopadCoreService;

    public MemopadMenuViewModel(IMemopadCoreService memopadCoreService)
    {
        _memopadCoreService = memopadCoreService ?? throw new ArgumentNullException(nameof(memopadCoreService));

        #region Model -> ViewModel -> View
        ShowStatusBar = MemopadCoreService.ShowStatusBar
            .Where(_ => memopadCoreService.CanNotification)
            .ToBindableReactiveProperty(true);
        IsWordWrap = MemopadCoreService.IsWordWrap
            .Where(_ => memopadCoreService.CanNotification)
            .ToBindableReactiveProperty(true);
        #endregion
    }
}
