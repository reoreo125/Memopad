using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using R3;

namespace Reoreo125.Memopad.Models.Services;

public interface IMemopadCoreService : IDisposable
{
    Observable<string> TextChanged { get; }
    Observable<bool> DirtyChanged { get; }
    Observable<string> TitleChanged { get; }
    Observable<(int, int)> SelectionChanged { get; }
    Observable<Encoding> EncodingChanged { get; }
    Observable<LineEnding> LineEndingChanged { get; }

    string Title { get; }
    string FilePath { get; }
    string FileName { get; }
    string FileNameWithoutExtension { get; }
    Encoding? Encoding { get; }
    string Text { get; }
    bool IsDirty { get; }
    int Row { get; }
    int Column { get; }
    FontFamily TextFont { get; }

    void Initialize();
    void NofityAllChanges();
    void LoadText(string filePath);
    void ChangeText(string newText, bool skipDirty = false);
    void ChangeSelection(int row, int column);
}

public sealed class MemopadCoreService : IMemopadCoreService
{
    private Subject<string> TextChangedSubject { get; set; } = new ();
    public Observable<string> TextChanged => TextChangedSubject;
    private Subject<bool> DirtyChangedSubject { get; set; } = new();
    public Observable<bool> DirtyChanged => DirtyChangedSubject;
    private Subject<string> TitleChangedSubject { get; set; } = new();
    public Observable<string> TitleChanged => TitleChangedSubject;
    private Subject<(int, int)> SelectionChangedSubject { get; set; } = new();
    public Observable<(int, int)> SelectionChanged => SelectionChangedSubject;
    private Subject<Encoding> EncodingChangedSubject { get; set; } = new();
    public Observable<Encoding> EncodingChanged => EncodingChangedSubject;
    private Subject<LineEnding> LineEndingChangedSubject { get; set; } = new();
    public Observable<LineEnding> LineEndingChanged => LineEndingChangedSubject;

    public string Title => $"{FileNameWithoutExtension}{(IsDirty ? "*" : "")} - {MemopadSettings.ApplicationName}";
    public string FileName => string.IsNullOrEmpty(FilePath) ? $"{MemopadSettings.DefaultNewFileName}.txt" : Path.GetFileName(FilePath);
    public string FileNameWithoutExtension => string.IsNullOrEmpty(FilePath) ? MemopadSettings.DefaultNewFileName : Path.GetFileNameWithoutExtension(FilePath);

    public string FilePath { get; private set; } = string.Empty;
    public Encoding? Encoding { get; private set; } = null;
    public LineEnding LineEnding { get; private set; } = LineEnding.Unknown;
    public string Text { get; private set; } = string.Empty;
    public string PreviousText { get; private set; } = string.Empty;
    public bool IsDirty { get; private set; } = false;
    public bool CanNotification { get; set; } = true;
    public int Row { get; set; } = 1;
    public int Column { get; set; } = 1;
    public FontFamily TextFont { get; set; } = FontFamily.GenericMonospace;

    private DisposableBag _disposableCollection = new();

    [Dependency]
    public ITextFileService? TextFileService { get; set; }

    public MemopadCoreService()
    {

    }

    public void EnableNotification()
    {
        CanNotification = true;
    }
    public void DisableNotification()
    {
        CanNotification = false;
    }
    public void Initialize()
    {
        FilePath = string.Empty;
        Encoding = MemopadSettings.DefaultEncoding;
        LineEnding = MemopadSettings.DefaultLineEnding;
        Text = string.Empty;
        PreviousText = string.Empty;
        IsDirty = false;
        Row = 1;
        Column = 1;

        if(CanNotification) NofityAllChanges();
    }
    public void NofityAllChanges()
    {
        if (!CanNotification) return;

        TextChangedSubject.OnNext(Text);
        DirtyChangedSubject.OnNext(IsDirty);
        SelectionChangedSubject.OnNext((Row, Column));
        EncodingChangedSubject.OnNext(Encoding ?? Encoding.UTF8);
        LineEndingChangedSubject.OnNext(LineEnding);
        TitleChangedSubject.OnNext(Title);
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

        Initialize();
        ChangeText(result.Content, true);
        ChangeEncoding(result.Encoding);
        ChangeLineEnding(result.LineEnding);
        FilePath = result.FilePath;

        EnableNotification();

        NofityAllChanges();
    }
    public void ChangeText(string newText, bool skipDirty = false)
    {
        PreviousText = Text;
        Text = newText;
        TextChangedSubject.OnNext(Text);

        if (!skipDirty && !IsDirty && PreviousText != Text)
        {
            IsDirty = true;
        }

        if(CanNotification) DirtyChangedSubject.OnNext(IsDirty);
    }
    public void ChangeSelection(int row, int column)
    {
        Row = row;
        Column = column;

        if(CanNotification) SelectionChangedSubject.OnNext((Row, Column));
    }
    public void ChangeEncoding(Encoding encoding)
    {
        Encoding = encoding;

        if(CanNotification) EncodingChangedSubject.OnNext(Encoding);
    }
    public void ChangeLineEnding(LineEnding lineEnding)
    {
        LineEnding = lineEnding;

        if(CanNotification) LineEndingChangedSubject.OnNext(LineEnding);
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
    public static LineEnding DefaultLineEnding => LineEnding.CRLF;
    public static Encoding DefaultEncoding => Encoding.UTF8;
    public static string DefaultEncodingText => DefaultEncoding.WebName.ToUpper();
    public static string DefaultPositionText => "1行 1列";
}
