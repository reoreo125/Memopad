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
    public ISaveTextFileCommand? SaveTextFileCommand { get; set; }
    [Dependency]
    public ISaveAsTextFileCommand? SaveAsTextFileCommand { get; set; }
    [Dependency]
    public IOpenNewWindowCommand? OpenNewWindowCommand { get; set; }

    public BindableReactiveProperty<bool> ShowStatusBar { get; }
    public BindableReactiveProperty<bool> IsWordWrap { get; }

    protected IEditorService EditorService => _editorService;
    private readonly IEditorService _editorService;

    public MemopadMenuViewModel(IEditorService editorService)
    {
        _editorService = editorService ?? throw new ArgumentNullException(nameof(EditorService));

        #region Model -> ViewModel -> View
        ShowStatusBar = EditorService.Settings.ShowStatusBar
            .ToBindableReactiveProperty(true);
        IsWordWrap = EditorService.Settings.IsWordWrap
            .ToBindableReactiveProperty(true);
        #endregion
    }
}
