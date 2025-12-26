using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Threading;
using R3;
using Reoreo125.Memopad.Models.History;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.Models;

public interface IEditorService : IDisposable
{
    public Observable<Unit> RequestCut { get; }
    public Observable<Unit> RequestCopy { get; }
    public Observable<Unit> RequestPaste { get; }
    public Observable<Unit> RequestDelete { get; }
    public Observable<string> RequestInsert { get; }
    public Observable<string> RequestLoadText { get; }
    public Observable<Unit> RequestReset { get; }

    public EditorDocument Document { get; }

    public void Reset();
    public void LoadText(string filePath);
    public void SaveText();
    public void SaveText(string filePath);
    public void Cut();
    public void Copy();
    public void Paste();
    public void Delete();
    public void Insert(string text);
}

public sealed class EditorService : IEditorService
{
    private readonly Subject<Unit> _requestCutSubject = new();
    public Observable<Unit> RequestCut => _requestCutSubject;
    private readonly Subject<Unit> _requestCopySubject = new ();
    public Observable<Unit> RequestCopy => _requestCopySubject;
    private readonly Subject<Unit> _requestPasteSubject = new ();
    public Observable<Unit> RequestPaste => _requestPasteSubject;
    private readonly Subject<Unit> _requestDeleteSubject = new ();
    public Observable<Unit> RequestDelete => _requestDeleteSubject;
    private readonly Subject<string> _requestInsertSubject = new();
    public Observable<string> RequestInsert => _requestInsertSubject;
    private readonly Subject<string> _requestLoadTextSubject = new();
    public Observable<string> RequestLoadText => _requestLoadTextSubject;
    private readonly Subject<Unit> _requestResetSubject = new();
    public Observable<Unit> RequestReset => _requestResetSubject;

    public EditorDocument Document { get; }

    private ITextFileService TextFileService { get; }

    private DisposableBag _disposableCollection = new();

    public EditorService(ITextFileService textFileService)
    {
        TextFileService = textFileService;

        Document = new EditorDocument();
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

        Reset();
        Document.FilePath.Value = result.FilePath;
        Document.BaseText.Value = result.Content;
        Document.Encoding.Value = result.Encoding;
        Document.HasBom.Value = result.HasBOM;
        // LineEnding が不明な場合はデフォルト値を使う
        Document.LineEnding.Value = (result.LineEnding is LineEnding.Unknown) ? Defaults.LineEnding : result.LineEnding;

        _requestLoadTextSubject.OnNext(result.Content);
    }
    public void SaveText() => SaveText(Document.FilePath.Value);
    public void SaveText(string filePath)
    {
        if (TextFileService is null) throw new Exception(nameof(TextFileService));

        var result = TextFileService.Save(filePath, Document.Text.Value, Document.Encoding.Value, Document.HasBom.Value, Document.LineEnding.Value);

        if (!result.IsSuccess)
        {
            MessageBox.Show("ファイルの保存に失敗しました。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Document.FilePath.Value = result.FilePath;
        Document.BaseText.Value = Document.Text.Value;
    }
    public void Reset()
    {
        Document.Text.Value = string.Empty;
        Document.BaseText.Value = string.Empty;
        Document.FilePath.Value = string.Empty;

        Document.Encoding.Value = Defaults.Encoding;
        Document.HasBom.Value = Defaults.HasBOM;
        Document.LineEnding.Value = Defaults.LineEnding;

        Document.CaretIndex.Value = 0;
        Document.SelectionLength.Value = 0;
        Document.Row.Value = 1;
        Document.Column.Value = 1;

        _requestResetSubject.OnNext(Unit.Default);
    }
    public void Cut()
    {
        _requestCutSubject.OnNext(Unit.Default);
    }
    public void Copy()
    {
        _requestCopySubject.OnNext(Unit.Default);
    }
    public void Paste()
    {
        _requestPasteSubject.OnNext(Unit.Default);
    }
    public void Delete()
    {
        _requestDeleteSubject.OnNext(Unit.Default);
    }
    public void Insert(string text)
    {
        _requestInsertSubject.OnNext(text);
    }

    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
