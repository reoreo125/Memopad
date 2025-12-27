using System.Data.Common;
using System.Windows;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using static System.Net.Mime.MediaTypeNames;

namespace Reoreo125.Memopad.ViewModels.Dialogs
{
    public class FindDialogViewModel : BindableBase, IDialogAware, IDisposable
    {
        public string? Title => "検索";

        public DialogCloseListener RequestClose { get; }

        // 各コマンド
        public DelegateCommand FindCommand => new(() =>
        {
            if (IsSearchUp.Value) FindPrevCommand!.Execute(null);
            else                  FindNextCommand!.Execute(null);
        });
        public DelegateCommand CancelCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));

        [Dependency]
        public IFindNextCommand? FindNextCommand { get; set; }
        [Dependency]
        public IFindPrevCommand? FindPrevCommand { get; set; }

        public BindableReactiveProperty<string> SearchText { get; }
        public BindableReactiveProperty<bool> MatchCase { get; }
        public BindableReactiveProperty<bool> WrapAround { get; }
        public BindableReactiveProperty<bool> IsSearchUp { get; }
        public BindableReactiveProperty<int> SearchTextMaxLength { get; }
        public IEditorService EditorService => _editorService;
        private readonly IEditorService _editorService;
        protected ISettingsService SettingsService => _settingsService;
        private readonly ISettingsService _settingsService;

        private DisposableBag _disposableCollection = new();

        public FindDialogViewModel(IEditorService editorService, ISettingsService settingsService)
        {
            _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(SettingsService));

            #region Model -> ViewModel -> View

            SearchText = EditorService.Document.SearchText
                .ToBindableReactiveProperty(string.Empty);
            MatchCase = EditorService.Document.MatchCase
                .ToBindableReactiveProperty(Defaults.MatchCase);
            WrapAround = EditorService.Document.WrapAround
                .ToBindableReactiveProperty(Defaults.WrapAround);
            IsSearchUp = new(false);
            SearchTextMaxLength = new(Defaults.SearchTextMaxLength);
            #endregion

            #region View -> ViewModel -> Model

            SearchText.Where(value => value is not null)
                      .Subscribe(value => EditorService.Document.SearchText.Value = value)
                      .AddTo(ref _disposableCollection);
            MatchCase.Subscribe(value => EditorService.Document.MatchCase.Value = value)
                      .AddTo(ref _disposableCollection);
            WrapAround.Subscribe(value => EditorService.Document.WrapAround.Value = value)
                      .AddTo(ref _disposableCollection);

            #endregion
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
