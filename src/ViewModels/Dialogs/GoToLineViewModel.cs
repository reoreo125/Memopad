using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.ViewModels.Windows;

public class GoToLineViewModel : BindableBase, IDialogAware
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

    public GoToLineViewModel(IEditorService editorService, IDialogService dialogService)
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
