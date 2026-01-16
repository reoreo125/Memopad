using R3;
using Reoreo125.Memopad.Models;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.ViewModels.Dialogs;

public class GoToLineDIalogViewModel : BindableBase, IDialogAware
{
    public string? Title => $"行へ移動";
    public DialogCloseListener RequestClose { get; }

    public DelegateCommand GoCommand => new(() =>
    {
        var result = EditorService.GoToLine(LineIndex.Value);
        if(result is false)
        {
            DialogService.ShowLineOutOfBounds();
            return;
        }

        RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
    });
    public DelegateCommand CancelCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));

    public BindableReactiveProperty<int> LineIndex { get; }

    public IEditorService EditorService => _editorService;
    private readonly IEditorService _editorService;
    public IDialogService DialogService => _dialogService;
    private readonly IDialogService _dialogService;

    public GoToLineDIalogViewModel(IEditorService editorService, IDialogService dialogService)
    {
        _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        LineIndex = new();
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        LineIndex.Value = parameters.GetValue<int>("lineIndex");
    }
    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
}
