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
}

public sealed class MemopadCoreService : IMemopadCoreService
{
    public ReactiveProperty<string> FilePath { get; private set; } = new();
    public ReactiveProperty<string> FileName { get; private set; } = new();
    public ReactiveProperty<string> FileNameWithoutExtension { get; private set; } = new();

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

        Text.Pairwise()
            .Subscribe(pair =>
            {
                if (CanCheckDirty && !IsDirty.Value && pair.Previous != pair.Current)
                {
                    IsDirty.Value = true;
                }
            })
            .AddTo(ref _disposableCollection);
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
        LineEnding.Value = result.LineEnding;
        FilePath.Value = result.FilePath;
        FileName.Value = string.IsNullOrEmpty(FilePath.Value) ? $"{MemoPadDefaults.NewFileName}.txt" : Path.GetFileName(FilePath.Value);
        FileNameWithoutExtension.Value = string.IsNullOrEmpty(FilePath.Value) ? MemoPadDefaults.NewFileName : Path.GetFileNameWithoutExtension(FilePath.Value);

        EnableNotification();
        EnableCheckDirty();

        NofityAllChanges();
    }
    public void SaveText(string filePath)
    {
        if (TextFileService is null) throw new Exception("TextFileService");
        var result = TextFileService.Save(this);

        if (!result.IsSuccess)
        {
            MessageBox.Show("ファイルの保存に失敗しました。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        IsDirty.Value = false;
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
public record MemoPadDefaults
{
    public static string ApplicationName => "Memopad";
    public static string NewFileName => "無題";
    public static string FileExtension => ".txt";
    public static LineEnding LineEnding => LineEnding.CRLF;
    public static Encoding Encoding => Encoding.UTF8;
    public static string EncodingText => Encoding.WebName.ToUpper();
    public static string PositionText => "1行、1列";
    public static FontFamily TextFont => FontFamily.GenericMonospace;
    public static double ZoomLevel => 1.0;
    public static double ZoomStep => 0.1;
    public static double ZoomMax => 5.0;
    public static double ZoomMin => 0.1;
    public static string ZoomLevelText => "100%";
    public static int FontSize => 12;
    public static TextWrapping TextWrapping => TextWrapping.NoWrap;
    public static int TextBoxDebounce => 500;
}
