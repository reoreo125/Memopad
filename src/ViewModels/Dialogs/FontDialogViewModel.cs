using System.Collections.ObjectModel;
using System.Windows.Media;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.TextProcessing;
using static Reoreo125.Memopad.Models.TextProcessing.Characterset;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.ViewModels.Dialogs;

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
            DialogService.ShowFontNotFound("その名前のフォントはありません。\nフォント一覧からフォントを選んでください。");
            return;
        }
        if (FontStyles.Select(value => value.Name).Contains(FontStyleInfo.Value.Name) is not true)
        {
            DialogService.ShowFontNotFound("そのスタイルでは、このフォントを利用できません。\nスタイル一覧からスタイルを選んでください。");
            return;
        }
        SettingsService.Settings.FontFamilyName.Value = FontName.Value;
        SettingsService.Settings.FontStyleName.Value = FontStyleInfo.Value.Name;
        SettingsService.Settings.FontStyleName.ForceNotify();
        SettingsService.Settings.FontSize.Value = Convert.ToInt32(Size.Value);
        RequestClose.Invoke(new DialogResult(ButtonResult.OK));
    });

    public BindableReactiveProperty<string> FontName { get; }
    public List<string> FontNames { get; }
    public BindableReactiveProperty<string> ListBoxFontName { get; }

    public BindableReactiveProperty<FontStyleInfo> FontStyleInfo { get; }
    public ObservableCollection<FontStyleInfo> FontStyles { get; }
    public BindableReactiveProperty<FontStyleInfo> ListBoxFontStyleInfo { get; }

    public BindableReactiveProperty<string> Size { get; }
    public BindableReactiveProperty<string?> ListBoxSize { get; }

    public BindableReactiveProperty<CharacterSet> CharacterSet { get; }
    public ObservableCollection<CharacterSet> AvailableCharacterSets { get; }
    public BindableReactiveProperty<string> SampleText { get; }
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

        // FontName ---
        FontName = new(SettingsService.Settings.FontFamilyName.Value);
        FontNames = Fonts.SystemFontFamilies
            .Select(f => f.Source)
            .OrderBy(n => n)
            .ToList();
        ListBoxFontName = new(FontName.Value);
        ListBoxFontName
            .Where(x => x != null)
            .Subscribe(x => FontName.Value = x)
            .AddTo(ref _disposableCollection);
        // ---

        // FontStyle
        FontStyles = new();
        var styles = Models.TextProcessing.FontStyleInfo.FromFontFamily(FontName.Value);
        FontStyleInfo = new();
        var styleInfo = styles.FirstOrDefault(value => value.Name == SettingsService.Settings.FontStyleName.Value);
        if (styleInfo is null) styleInfo = styles.First();
        FontStyleInfo.Value = styleInfo;
        ListBoxFontStyleInfo = new();
        
        FontName.Subscribe(name =>
                {
                    if (string.IsNullOrEmpty(name)) return;

                    bool isFirst = false;
                    if (FontStyles.Count == 0) isFirst = true;

                    var styles = Models.TextProcessing.FontStyleInfo.FromFontFamily(name);

                    FontStyles.Clear();
                    foreach (var style in styles) FontStyles.Add(style);

                    if (isFirst)
                    {
                        ListBoxFontStyleInfo.Value = FontStyleInfo.Value;
                    }
                    else
                    {
                        var styleInfo = styles.FirstOrDefault(value => value.Name == SettingsService.Settings.FontStyleName.Value);
                        if (styleInfo is null) styleInfo = styles.First();
                        ListBoxFontStyleInfo.Value = styleInfo;
                    }
                })
                .AddTo(ref _disposableCollection);
        ListBoxFontStyleInfo
            .Where(value => value != null)
            .Subscribe(value =>
            {
                FontStyleInfo.Value = value;
            })
            .AddTo(ref _disposableCollection);
        FontStyleInfo
            .Subscribe(value =>
            {
                ListBoxFontStyleInfo.Value = value;
            })
            .AddTo (ref _disposableCollection);
        // ---

        // FontSize ---
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

        // SampleText
        AvailableCharacterSets = new();
        CharacterSet = new();
        SampleText = new();
        FontName.Subscribe(name =>
            {
                AvailableCharacterSets.Clear();
                foreach (var set in AllCharacterSets.Where(cs => IsFontSupportCharacterSet(FontName.Value, cs)))
                {
                    AvailableCharacterSets.Add(set);
                }
                CharacterSet.Value = AvailableCharacterSets.First();
            })
            .AddTo(ref _disposableCollection);
        CharacterSet.Subscribe(charSet =>
            {
                if (charSet is null) return;

                SampleText.Value = charSet.SampleText;
            })
            .AddTo(ref _disposableCollection);
        // ---
    }
    private bool IsFontSupportCharacterSet(string fontName, CharacterSet set)
    {
        // 欧文は基本サポートとみなす（または判定をスキップ）
        if (set.CharSet == GdiCharSet.Ansi) return true;

        try
        {
            var family = new System.Windows.Media.FontFamily(fontName);
            var typeface = family.GetTypefaces().FirstOrDefault();
            if (typeface == null) return false;

            if (typeface.TryGetGlyphTypeface(out var glyph))
            {
                // CharacterToGlyphMap に判定用文字の Unicode (int) が含まれているか確認
                return glyph.CharacterToGlyphMap.ContainsKey(set.CheckChar);
            }
        }
        catch
        {
            return false;
        }
        return false;
    }

    public void OnDialogOpened(IDialogParameters parameters) {}
    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
