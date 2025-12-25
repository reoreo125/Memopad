using System.IO;
using System.Text;
using System.Windows;
using R3;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.Models;

public interface ICoreService : IDisposable
{
    public ReactiveProperty<string> FilePath { get; }
    public ReactiveProperty<string> FileName { get; }
    public ReactiveProperty<string> FileNameWithoutExtension { get; }

    public ReactiveProperty<string> Title { get; }
    public ReactiveProperty<string> Text { get; }

    public ReactiveProperty<Encoding> Encoding { get; }
    public ReactiveProperty<bool> HasBom { get; }
    public ReactiveProperty<LineEnding> LineEnding { get; }
    public ReactiveProperty<bool> IsDirty { get; }
    public ReactiveProperty<int> Row { get; }
    public ReactiveProperty<int> Column { get; }

    public ReactiveProperty<DateTime> InsertDateTime { get; }

    public ReactiveProperty<int> CaretIndex { get; }
    public ReactiveProperty<int> SelectionLength { get; }

    public bool CanNotification { get; }

    public Settings Settings { get; }

    public void Initialize();
    public void NofityAllChanges();
    public void LoadText(string filePath);
    public void SaveText();
    public void SaveText(string filePath);
}

public sealed class CoreService : ICoreService
{
    public ReactiveProperty<string> FilePath { get; } = new();
    public ReactiveProperty<string> FileName { get; } = new();
    public ReactiveProperty<string> FileNameWithoutExtension { get; } = new();

    public ReactiveProperty<string> Title { get; } = new();
    public ReactiveProperty<string> Text { get; } = new();
    public string PreviousText { get; private set; } = string.Empty;

    public ReactiveProperty<bool> IsDirty { get; } = new ReactiveProperty<bool>(false);
    public bool CanNotification { get; set; } = true;
    public bool CanCheckDirty { get; set; } = true;

    public ReactiveProperty<int> Row { get; } = new(1);
    public ReactiveProperty<int> Column { get; } = new(1);
    public ReactiveProperty<Encoding> Encoding { get; } = new();
    public ReactiveProperty<bool> HasBom { get; } = new();
    public ReactiveProperty<LineEnding> LineEnding { get; } = new();

    public ReactiveProperty<DateTime> InsertDateTime { get; } = new(DateTime.MinValue);

    public ReactiveProperty<int> CaretIndex { get; } = new(0);
    public ReactiveProperty<int> SelectionLength { get; } = new(0);

    private DisposableBag _disposableCollection = new();


    

    public Settings Settings => MemopadSettingsService.Settings;
    private ISettingsService MemopadSettingsService { get; }
    private ITextFileService? TextFileService { get; }

    public CoreService(ISettingsService settingsService, ITextFileService textFileService)
    {
        MemopadSettingsService = settingsService;
        TextFileService = textFileService;

        Initialize();

        // タイトルを作るためのインナーメソッド
        string CreateTitle() => $"{(IsDirty.Value ? "*" : "")}{FileNameWithoutExtension.Value} - {Defaults.ApplicationName}";

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
        FileName.Value = Defaults.NewFileName + Defaults.FileExtension;
        FileNameWithoutExtension.Value = Defaults.NewFileName;

        Text.Value = string.Empty;
        PreviousText = string.Empty;

        Encoding.Value = Defaults.Encoding;
        HasBom.Value = Defaults.HasBOM;
        LineEnding.Value = Defaults.LineEnding;
        
        IsDirty.Value = false;
        Row.Value = 1;
        Column.Value = 1;

        NofityAllChanges();
    }
    public void NofityAllChanges()
    {
        if (!CanNotification) return;

        Title.ForceNotify();
        Text.ForceNotify();
        IsDirty.ForceNotify();
        Row.ForceNotify();
        Column.ForceNotify();
        Encoding.ForceNotify();
        HasBom.ForceNotify();
        LineEnding.ForceNotify();
        IsDirty.ForceNotify();

        Settings.ShowStatusBar.ForceNotify();
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
        HasBom.Value = result.HasBOM;
        // LineEnding が不明な場合はデフォルト値を使う
        LineEnding.Value = (result.LineEnding is TextProcessing.LineEnding.Unknown) ? Defaults.LineEnding : result.LineEnding;
        FilePath.Value = result.FilePath;
        FileName.Value = string.IsNullOrEmpty(FilePath.Value) ? $"{Defaults.NewFileName}.txt" : Path.GetFileName(FilePath.Value);
        FileNameWithoutExtension.Value = string.IsNullOrEmpty(FilePath.Value) ? Defaults.NewFileName : Path.GetFileNameWithoutExtension(FilePath.Value);

        EnableNotification();
        EnableCheckDirty();

        NofityAllChanges();
    }
    public void SaveText() => SaveText(FilePath.Value);
    public void SaveText(string filePath)
    {
        if (TextFileService is null) throw new Exception(nameof(TextFileService));
        if (FilePath is null) return;

        var result = TextFileService.Save(filePath, Text.Value, Encoding.Value, HasBom.Value, LineEnding.Value);

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
