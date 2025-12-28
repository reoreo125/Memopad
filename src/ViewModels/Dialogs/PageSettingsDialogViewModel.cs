using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.ViewModels.Dialogs
{
    public class PageSettingsDialogViewModel : BindableBase, IDialogAware, IDisposable
    {
        public string? Title => "ページ設定";

        public DialogCloseListener RequestClose { get; }

        public DelegateCommand CancelCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));

        public IEditorService EditorService => _editorService;
        private readonly IEditorService _editorService;
        protected ISettingsService SettingsService => _settingsService;
        private readonly ISettingsService _settingsService;

        private DisposableBag _disposableCollection = new();

        public PageSettingsDialogViewModel(IEditorService editorService, ISettingsService settingsService)
        {
            _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(SettingsService));

        }
        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
        public void Dispose()
        {
            _disposableCollection.Dispose();
        }
    }
}
