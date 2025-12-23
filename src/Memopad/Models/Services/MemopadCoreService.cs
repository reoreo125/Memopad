using System.Drawing;
using System.IO;
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

    void ChangeText(string newText);
}

public class MemopadCoreService : IMemopadCoreService
{
    private Subject<string> TextChangedSubject { get; set; } = new ();
    public Observable<string> TextChanged => TextChangedSubject;
    private Subject<bool> DirtyChangedSubject { get; set; } = new();
    public Observable<bool> DirtyChanged => DirtyChangedSubject;

    public string Title => $"{FileNameWithoutExtension}{(IsDirty ? "*" : "")} - Memopad";
    public string FilePath { get; set; } = string.Empty;
    public string FileName => string.IsNullOrEmpty(FilePath) ? $"{_newFileName}.txt" : Path.GetFileName(FilePath);
    public string FileNameWithoutExtension => string.IsNullOrEmpty(FilePath) ? _newFileName : Path.GetFileNameWithoutExtension(FilePath);
    public Encoding? Encoding { get; set; } = null;
    public string Text { get; set; } = string.Empty;
    public string PreviousText { get; set; } = string.Empty;
    public bool IsDirty { get; protected set; } = false;
    public int Row { get; set; } = 0;
    public int Column { get; set; } = 0;
    public FontFamily TextFont { get; set; } = FontFamily.GenericMonospace;

    private const string _newFileName = "新規テキスト";

    private DisposableBag _disposableCollection = new();

    public MemopadCoreService()
    {

    }

    public void ChangeText(string newText)
    {
        PreviousText = Text;
        Text = newText;
        TextChangedSubject.OnNext(Text);

        if (!IsDirty && PreviousText != Text)
        {
            IsDirty = true;
            DirtyChangedSubject.OnNext(IsDirty);
        }
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
