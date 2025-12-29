using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.ViewModels.Dialogs
{
    public class FontDialogViewModel : BindableBase, IDialogAware, IDisposable
    {
        public string? Title => "フォント";

        public DialogCloseListener RequestClose { get; }

        public DelegateCommand CancelCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));
        public DelegateCommand OkCommand => new(() =>
        {
            // もし選択したフォント名がフォントリストになかったらダイアログを出してOKさせない
            if(FontNames.Contains(FontName.Value) is not true)
            {
                // DialogService.ShowFontNotFound();
                return;
            }

            SettingsService.Settings.FontFamilyName.Value = FontName.Value;
            SettingsService.Settings.FontSize.Value = Convert.ToInt32(Size.Value);
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        });

        public BindableReactiveProperty<string> FontName { get; }
        public BindableReactiveProperty<string> Size { get; }
        public BindableReactiveProperty<string?> ListBoxSize { get; }

        public BindableReactiveProperty<string> ListBoxFontName { get; }
        public List<string> FontNames { get; }
        public string[] PresetSizes { get; } = new[]
        { "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72" };


        public IEditorService EditorService => _editorService;
        private readonly IEditorService _editorService;
        protected ISettingsService SettingsService => _settingsService;
        private readonly ISettingsService _settingsService;
        protected IDialogService DialogService => _dialogService;
        private readonly IDialogService _dialogService;

        private DisposableBag _disposableCollection = new();

        public FontDialogViewModel(IEditorService editorService, ISettingsService settingsService, IDialogService dialogService)
        {
            _editorService = editorService ?? throw new ArgumentNullException(nameof(EditorService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(SettingsService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(DialogService));

            // フォント名 ---
            FontName = new(SettingsService.Settings.FontFamilyName.Value);
            // システムフォント名を取得してソート
            FontNames = Fonts.SystemFontFamilies
                .Select(f => f.Source)
                .OrderBy(n => n)
                .ToList();
            // ListBoxで選ばれたらTextBoxと結果に反映
            ListBoxFontName = new(FontName.Value);
            ListBoxFontName
                .Where(x => x != null)
                .Subscribe(x => FontName.Value = x)
                .AddTo(ref _disposableCollection);
            // ---

            // フォントサイズ ---
            Size = new(SettingsService.Settings.FontSize.Value.ToString());
            ListBoxSize = new(SettingsService.Settings.FontSize.Value.ToString());
            ListBoxSize
                .Where(x => x != null)
                .Subscribe(x => Size.Value = x!)
                .AddTo(ref _disposableCollection);
            Size
                .Subscribe(x => {
                    if (string.IsNullOrEmpty(x)) return;

                    if (int.TryParse(x, out int val) && val <= 0) Size.Value = "1";

                    if (PresetSizes.Contains(x)) ListBoxSize.Value = x;
                    else ListBoxSize.Value = null; // リストにない数値なら選択解除
                })
                .AddTo(ref _disposableCollection);
            // ---
        }
        public void OnDialogOpened(IDialogParameters parameters) {}
        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
        public void Dispose()
        {
            _disposableCollection.Dispose();
        }
    }
}
