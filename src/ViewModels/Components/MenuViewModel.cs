using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.ViewModels.Components;

public class MenuViewModel : BindableBase
{
    [Dependency]
    public IExitCommand? ApplicationExitCommand { get; set; }
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
    [Dependency]
    public ICutCommand? CutCommand { get; set; }
    [Dependency]
    public ICopyCommand? CopyCommand { get; set; }
    [Dependency]
    public IPasteCommand? PasteCommand { get; set; }
    [Dependency]
    public IDeleteCommand? DeleteCommand { get; set; }
    [Dependency]
    public IOpenPrintCommand? OpenPrintCommand { get; set; }
    [Dependency]
    public IUndoCommand? UndoCommand { get; set; }
    [Dependency]
    public IRedoCommand? RedoCommand { get; set; }
    [Dependency]
    public IOpenFindCommand? OpenFindCommand { get; set; }
    [Dependency]
    public IFindNextCommand? FindNextCommand { get; set; }
    [Dependency]
    public IFindPrevCommand? FindPrevCommand { get; set; }
    [Dependency]
    public ISelectAllCommand? SelectAllCommand { get; set; }
    [Dependency]
    public IOpenGoToLineCommand? OpenGoToLineCommand { get; set; }
    [Dependency]
    public IOpenReplaceCommand? OpenReplaceCommand { get; set; }

    public BindableReactiveProperty<bool> ShowStatusBar { get; }
    public BindableReactiveProperty<bool> IsWordWrap { get; }

    protected IEditorService EditorService => _editorService;
    private readonly IEditorService _editorService;
    protected ISettingsService SettingsService => _settingsService;
    private readonly ISettingsService _settingsService;

    public MenuViewModel(IEditorService editorService, ISettingsService settingsService)
    {
        _editorService = editorService ?? throw new ArgumentNullException(nameof(EditorService));
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(SettingsService));

        #region Model -> ViewModel -> View
        ShowStatusBar = SettingsService.Settings.ShowStatusBar
            .ToBindableReactiveProperty(true);
        IsWordWrap = SettingsService.Settings.IsWordWrap
            .ToBindableReactiveProperty(true);
        #endregion
    }
}
