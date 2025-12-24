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
    FontFamily TextFont { get; }

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
    public ReactiveProperty<int> Row { get; set; } = new(1);
    public ReactiveProperty<int> Column { get; set; } = new(1);


    public ReactiveProperty<Encoding?> Encoding { get; private set; } = new();
    public ReactiveProperty<LineEnding> LineEnding { get; private set; } = new();
    public FontFamily TextFont { get; set; } = FontFamily.GenericMonospace;

    private DisposableBag _disposableCollection = new();

    [Dependency]
    public ITextFileService? TextFileService { get; set; }

    public MemopadCoreService()
    {
        Initialize();

        Text.Pairwise()
            .Where(_ => CanNotification)
            .Subscribe(pair =>
            {
                if (CanCheckDirty && !IsDirty.Value && pair.Previous != pair.Current)
                {
                    IsDirty.Value = true;
                }
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
        FileName.Value = MemopadSettings.DefaultNewFileName + MemopadSettings.DefaultFileExtension;
        FileNameWithoutExtension.Value = MemopadSettings.DefaultNewFileName;

        Text.Value = string.Empty;
        PreviousText = string.Empty;

        Encoding.Value = null;
        LineEnding.Value = MemopadSettings.DefaultLineEnding;
        
        
        IsDirty.Value = false;
        Row.Value = 1;
        Column.Value = 1;
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
        FileName.Value = string.IsNullOrEmpty(FilePath.Value) ? $"{MemopadSettings.DefaultNewFileName}.txt" : Path.GetFileName(FilePath.Value);
        FileNameWithoutExtension.Value = string.IsNullOrEmpty(FilePath.Value) ? MemopadSettings.DefaultNewFileName : Path.GetFileNameWithoutExtension(FilePath.Value);

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

    public static string ApplicationName => "Memopad";
    public static string DefaultNewFileName => "新規テキスト";
    public static string DefaultFileExtension => ".txt";
    public static LineEnding DefaultLineEnding => LineEnding.CRLF;
    public static Encoding DefaultEncoding => Encoding.UTF8;
    public static string DefaultEncodingText => DefaultEncoding.WebName.ToUpper();
    public static string DefaultPositionText => "1行 1列";
}
