using System.Data.Common;
using System.Windows;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using static System.Net.Mime.MediaTypeNames;

namespace Reoreo125.Memopad.ViewModels.Dialogs
{
    public class ReplaceDialogViewModel : BindableBase, IDialogAware, IDisposable
    {
        public string? Title => "置換";

        public DialogCloseListener RequestClose { get; }

        [Dependency]
        public IFindNextCommand? FindNextCommand { get; set; }
        [Dependency]
        public IReplaceNextCommand? ReplaceNextCommand { get; set; }
        [Dependency]
        public IReplaceAllCommand? ReplaceAllCommand { get; set; }
        public DelegateCommand CancelCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));

        public BindableReactiveProperty<string> SearchText { get; }
        public BindableReactiveProperty<string> ReplaceText { get; }

        public BindableReactiveProperty<bool> MatchCase { get; }
        public BindableReactiveProperty<bool> WrapAround { get; }

        public BindableReactiveProperty<bool> FindNextButton_IsEnabled { get; }
        public BindableReactiveProperty<bool> ReplaceNextButton_IsEnabled { get; }
        public BindableReactiveProperty<bool> ReplaceAllButton_IsEnabled { get; }

        public IEditorService EditorService => _editorService;
        private readonly IEditorService _editorService;
        protected ISettingsService SettingsService => _settingsService;
        private readonly ISettingsService _settingsService;

        private DisposableBag _disposableCollection = new();

        public ReplaceDialogViewModel(IEditorService editorService, ISettingsService settingsService)
        {
            _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(SettingsService));

            #region Model -> ViewModel -> View

            SearchText = EditorService.Document.SearchText
                .ToBindableReactiveProperty(string.Empty);
            ReplaceText = EditorService.Document.ReplaceText
                .ToBindableReactiveProperty(string.Empty);
            MatchCase = EditorService.Document.MatchCase
                .ToBindableReactiveProperty(Defaults.MatchCase);
            WrapAround = EditorService.Document.WrapAround
                .ToBindableReactiveProperty(Defaults.WrapAround);

            FindNextButton_IsEnabled = EditorService.Document.SearchText
                .Select(value => !string.IsNullOrEmpty(value))
                .ToBindableReactiveProperty(false);
            ReplaceNextButton_IsEnabled = EditorService.Document.SearchText
                .Select(value => !string.IsNullOrEmpty(value))
                .ToBindableReactiveProperty(false);
            ReplaceAllButton_IsEnabled = EditorService.Document.SearchText
                .Select(value => !string.IsNullOrEmpty(value))
                .ToBindableReactiveProperty(false);

            #endregion

            #region View -> ViewModel -> Model

            SearchText.Where(value => value is not null)
                .Subscribe(value => EditorService.Document.SearchText.Value = value)
                .AddTo(ref _disposableCollection);
            ReplaceText.Where(value => value is not null)
                .Subscribe(value => EditorService.Document.ReplaceText.Value = value)
                .AddTo(ref _disposableCollection);
            MatchCase.Subscribe(value => EditorService.Document.MatchCase.Value = value)
                .AddTo(ref _disposableCollection);
            WrapAround.Subscribe(value => EditorService.Document.WrapAround.Value = value)
                .AddTo(ref _disposableCollection);

            #endregion
        }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            SearchText.Value = EditorService.Document.SelectedText.Value;
        }
        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
        public void Dispose()
        {
            _disposableCollection.Dispose();
        }
    }
}
