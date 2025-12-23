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
    void ChangeText(string newText, bool skipDirty = false);
    void LoadText(string filePath);
    void ChangeSelection(int row, int column);
}

public sealed class MemopadCoreService : IMemopadCoreService
{
    private Subject<string> TextChangedSubject { get; set; } = new ();
    public Observable<string> TextChanged => TextChangedSubject;
    private Subject<bool> DirtyChangedSubject { get; set; } = new();
    public Observable<bool> DirtyChanged => DirtyChangedSubject;
    private Subject<(int, int)> SelectionChangedSubject { get; set; } = new();
    public Observable<(int, int)> SelectionChanged => SelectionChangedSubject;
    private Subject<Encoding> EncodingChangedSubject { get; set; } = new();
    public Observable<Encoding> EncodingChanged => EncodingChangedSubject;
    private Subject<LineEnding> LineEndingChangedSubject { get; set; } = new();
    public Observable<LineEnding> LineEndingChanged => LineEndingChangedSubject;

    public string Title => $"{FileNameWithoutExtension}{(IsDirty ? "*" : "")} - Memopad";
    public string FileName => string.IsNullOrEmpty(FilePath) ? $"{_newFileName}.txt" : Path.GetFileName(FilePath);
    public string FileNameWithoutExtension => string.IsNullOrEmpty(FilePath) ? _newFileName : Path.GetFileNameWithoutExtension(FilePath);

    public string FilePath { get; private set; } = string.Empty;
    public Encoding? Encoding { get; private set; } = null;
    public LineEnding LineEnding { get; private set; } = LineEnding.Unknown;
    public string Text { get; private set; } = string.Empty;
    public string PreviousText { get; private set; } = string.Empty;
    public bool IsDirty { get; private set; } = false;
    public int Row { get; set; } = 1;
    public int Column { get; set; } = 1;
    public FontFamily TextFont { get; set; } = FontFamily.GenericMonospace;

    private const string _newFileName = "新規テキスト";

    private DisposableBag _disposableCollection = new();

    [Dependency]
    public ITextFileService? TextFileService { get; set; }

    public MemopadCoreService()
    {

    }

    public void Initialize()
    {
        FilePath = string.Empty;
        Encoding = null;
        LineEnding = LineEnding.Unknown;
        Text = string.Empty;
        PreviousText = string.Empty;
        IsDirty = false;
        Row = 1;
        Column = 1;
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

        DirtyChangedSubject.OnNext(IsDirty);
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

        Initialize();
        ChangeText(result.Content, true);
        ChangeEncoding(result.Encoding);
        ChangeLineEnding(result.LineEnding);

        FilePath = result.FilePath;
    }
    public void ChangeSelection(int row, int column)
    {
        Row = row;
        Column = column;
        SelectionChangedSubject.OnNext((Row, Column));
    }
    public void ChangeEncoding(Encoding encoding)
    {
        Encoding = encoding;
        EncodingChangedSubject.OnNext(Encoding);
    }
    public void ChangeLineEnding(LineEnding lineEnding)
    {
        LineEnding = lineEnding;
        LineEndingChangedSubject.OnNext(LineEnding);
    }
    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}

public record MemopadSettings
{
    public string LastOpenedFolder { get; set; } = string.Empty;
}
