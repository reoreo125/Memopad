using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using R3;

namespace Reoreo125.Memopad.Models.Services;

public interface IMemopadCoreService : IDisposable
{
    ReactiveProperty<string> FilePath { get; }
    ReactiveProperty<string> FileName { get; }
    ReactiveProperty<string> FileNameWithoutExtension { get; }

    ReactiveProperty<string> Title { get; }
    ReactiveProperty<string> Text { get; }

    ReactiveProperty<Encoding?> Encoding { get; }
    ReactiveProperty<LineEnding> LineEnding { get; }
    ReactiveProperty<bool> IsDirty { get; }
    ReactiveProperty<int> Row { get; }
    ReactiveProperty<int> Column { get; }
    ReactiveProperty<double> ZoomLevel { get; }
    ReactiveProperty<bool> ShowStatusBar { get; }
    ReactiveProperty<bool> IsWordWrap { get; }

    ReactiveProperty<DateTime> InsertDateTime { get; }

    ReactiveProperty<int> CaretIndex { get; }
    ReactiveProperty<int> SelectionLength { get; }

    bool CanNotification { get; }

    void Initialize();
    void NofityAllChanges();
    void LoadText(string filePath);
    void SaveText();
    void SaveText(string filePath);
}

public sealed class MemopadCoreService : IMemopadCoreService
{
    public ReactiveProperty<string> FilePath { get; private set; } = new();
    public ReactiveProperty<string> FileName { get; private set; } = new();
    public ReactiveProperty<string> FileNameWithoutExtension { get; private set; } = new();

    public ReactiveProperty<string> Title { get; private set; } = new();
    public ReactiveProperty<string> Text { get; private set; } = new();
    public string PreviousText { get; private set; } = string.Empty;

    public ReactiveProperty<bool> IsDirty { get; } = new ReactiveProperty<bool>(false);
    public bool CanNotification { get; set; } = true;
    public bool CanCheckDirty { get; set; } = true;

    public ReactiveProperty<bool> ShowStatusBar { get; set; } = new(true);
    public ReactiveProperty<int> Row { get; set; } = new(1);
    public ReactiveProperty<int> Column { get; set; } = new(1);
    public ReactiveProperty<double> ZoomLevel { get; set; } = new(1.0);
    public ReactiveProperty<Encoding?> Encoding { get; private set; } = new();
    public ReactiveProperty<LineEnding> LineEnding { get; private set; } = new();
    public ReactiveProperty<bool> IsWordWrap { get; set; } = new(false);

    public ReactiveProperty<DateTime> InsertDateTime { get; } = new(DateTime.MinValue);

    public ReactiveProperty<int> CaretIndex { get; } = new(0);
    public ReactiveProperty<int> SelectionLength { get; } = new(0);

    private DisposableBag _disposableCollection = new();
    
    [Dependency]
    public ITextFileService? TextFileService { get; set; }

    public MemopadCoreService()
    {
        Initialize();

        // タイトルを作るためのインナーメソッド
        string CreateTitle() => $"{(IsDirty.Value ? "*" : "")}{FileNameWithoutExtension.Value} - {MemoPadDefaults.ApplicationName}";

        // テキスト変更フラグが変化するたびにタイトルを更新
        IsDirty.Subscribe(_ =>
            {
                Title.Value = CreateTitle();
                Title.ForceNotify(); // なぜか変化が伝わらないことがあるので強制通知
            })
               .AddTo(ref _disposableCollection);

        // ファイル名が変化するたびにタイトルを更新
        FileName.Subscribe(_ =>
            {
                Title.Value = CreateTitle();
                Title.ForceNotify();
            })
                .AddTo(ref _disposableCollection);

        // テキスト内容が変化したら変更フラグを立てる
        Text.Pairwise()
            .Subscribe(pair =>
            {
                if (CanCheckDirty && !IsDirty.Value && pair.Previous != pair.Current)
                {
                    IsDirty.Value = true;
                }
            })
            .AddTo(ref _disposableCollection);

        // 日付挿入要求が来たらテキストに日付を挿入してキャレット位置を更新
        InsertDateTime.Subscribe(value =>
            {
                if (value == DateTime.MinValue) return;

                var now = DateTime.Now.ToString("H:mm yyyy/MM/dd");
                var currentText = Text.Value ?? ""; // 生成後は最新の Value が取れる
                var start = CaretIndex.Value;
                var length = SelectionLength.Value;

                // 文字列挿入
                var newText = currentText.Remove(start, length).Insert(start, now);

                Text.Value = newText;

                CaretIndex.Value = start + now.Length;
            })
            .AddTo(ref _disposableCollection);
    }

    public void EnableNotification() => CanNotification = true;
    public void DisableNotification() => CanNotification = false;
    private void EnableCheckDirty() => CanCheckDirty = true;
    private void DisableCheckDirty() => CanCheckDirty = false;

    public void Initialize()
    {
        FilePath.Value = string.Empty;
        FileName.Value = MemoPadDefaults.NewFileName + MemoPadDefaults.FileExtension;
        FileNameWithoutExtension.Value = MemoPadDefaults.NewFileName;

        Text.Value = string.Empty;
        PreviousText = string.Empty;

        Encoding.Value = MemoPadDefaults.Encoding;
        LineEnding.Value = MemoPadDefaults.LineEnding;
        
        
        IsDirty.Value = false;
        Row.Value = 1;
        Column.Value = 1;
        ZoomLevel.Value = MemoPadDefaults.ZoomLevel;
    }
    public void NofityAllChanges()
    {
        if (!CanNotification) return;

        Text.ForceNotify();
        IsDirty.ForceNotify();
        Row.ForceNotify();
        Column.ForceNotify();
        Encoding.ForceNotify();
        LineEnding.ForceNotify();
        ShowStatusBar.ForceNotify();
        IsDirty.ForceNotify();
    }
    public void LoadText(string filePath)
    {
        if (TextFileService is null) throw new Exception("TextFileService");
        var result = TextFileService.Load(filePath);

        if (!result.IsSuccess)
        {
            MessageBox.Show("ファイルの読み込みに失敗しました。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        DisableNotification();
        DisableCheckDirty();

        Initialize();
        Text.Value = result.Content;
        Encoding.Value = result.Encoding;
        // LineEnding が不明な場合はデフォルト値を使う
        LineEnding.Value = (result.LineEnding is Services.LineEnding.Unknown) ? MemoPadDefaults.LineEnding : result.LineEnding;
        FilePath.Value = result.FilePath;
        FileName.Value = string.IsNullOrEmpty(FilePath.Value) ? $"{MemoPadDefaults.NewFileName}.txt" : Path.GetFileName(FilePath.Value);
        FileNameWithoutExtension.Value = string.IsNullOrEmpty(FilePath.Value) ? MemoPadDefaults.NewFileName : Path.GetFileNameWithoutExtension(FilePath.Value);

        EnableNotification();
        EnableCheckDirty();

        NofityAllChanges();
    }
    public void SaveText() => SaveText(FilePath.Value);
    public void SaveText(string filePath)
    {
        if (TextFileService is null) throw new Exception(nameof(TextFileService));
        if (FilePath is null) return;

        var result = TextFileService.Save(filePath, Text.Value, Encoding.Value, LineEnding.Value);

        if (!result.IsSuccess)
        {
            MessageBox.Show("ファイルの保存に失敗しました。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        DisableNotification();
        DisableCheckDirty();

        FilePath.Value = result.FilePath;
        FileName.Value = Path.GetFileName(FilePath.Value);
        FileNameWithoutExtension.Value = Path.GetFileNameWithoutExtension(FilePath.Value);

        IsDirty.Value = false;

        EnableNotification();
        EnableCheckDirty();

        NofityAllChanges();
    }
    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}

public record MemopadSettings
{
    public string LastOpenedFolder { get; set; } = string.Empty;
    public FontFamily TextFont { get; set; } = FontFamily.GenericMonospace;
}
