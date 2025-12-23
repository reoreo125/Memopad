using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using R3;
using Reoreo125.Memopad.ViewModels.Windows;

namespace Reoreo125.Memopad.Models.Services;

public interface IMemopadCoreService : IDisposable
{
    Observable<string> TextChanged { get; }
    Observable<bool> DirtyChanged { get; }
    Observable<(int, int)> SelectionChanged { get; }
    string Title { get; }
    string FilePath { get; set; }
    string FileName { get; }
    string FileNameWithoutExtension { get; }
    Encoding? Encoding { get; set; }
    string Text { get; }
    bool IsDirty { get; }
    int Row { get; set; }
    int Column { get; set; }
    FontFamily TextFont { get; set; }

    void Initialize();
    void ChangeText(string newText, bool skipDirty = false);
    void LoadText(string filePath);
    void ChangeSelection(int row, int column);
}

public class MemopadCoreService : IMemopadCoreService
{
    private Subject<string> TextChangedSubject { get; set; } = new ();
    public Observable<string> TextChanged => TextChangedSubject;
    private Subject<bool> DirtyChangedSubject { get; set; } = new();
    public Observable<bool> DirtyChanged => DirtyChangedSubject;
    private Subject<(int, int)> SelectionChangedSubject { get; set; } = new();
    public Observable<(int, int)> SelectionChanged => SelectionChangedSubject;


    public string Title => $"{FileNameWithoutExtension}{(IsDirty ? "*" : "")} - Memopad";
    public string FileName => string.IsNullOrEmpty(FilePath) ? $"{_newFileName}.txt" : Path.GetFileName(FilePath);
    public string FileNameWithoutExtension => string.IsNullOrEmpty(FilePath) ? _newFileName : Path.GetFileNameWithoutExtension(FilePath);

    public string FilePath { get; set; } = string.Empty;
    public Encoding? Encoding { get; set; } = null;
    public string Text { get; set; } = string.Empty;
    public string PreviousText { get; set; } = string.Empty;
    public bool IsDirty { get; protected set; } = false;
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
    }
    public void ChangeSelection(int row, int column)
    {
        Row = row;
        Column = column;
        SelectionChangedSubject.OnNext((Row, Column));
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
