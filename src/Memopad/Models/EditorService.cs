using System.Windows;
using System.Windows.Documents;
using R3;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.Models;

public interface IEditorService : IDisposable
{
    public Observable<Unit> RequestReset { get; }
    public Observable<string> RequestLoadText { get; }
    public Observable<Unit> RequestCut { get; }
    public Observable<Unit> RequestCopy { get; }
    public Observable<Unit> RequestPaste { get; }
    public Observable<Unit> RequestDelete { get; }
    public Observable<string> RequestInsert { get; }
    public Observable<Unit> RequestUndo { get; }
    public Observable<Unit> RequestRedo { get; }
    public Observable<(int foundIndex, int length)> RequestSelect { get; }

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
    public void Undo();
    public void Redo();
    public bool Find(string target, bool matchCase, bool wrapAround, bool searchUp);
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
    private readonly Subject<Unit> _requestUndoSubject = new();
    public Observable<Unit> RequestUndo => _requestUndoSubject;
    private readonly Subject<Unit> _requestRedoSubject = new();
    public Observable<Unit> RequestRedo => _requestRedoSubject;
    private readonly Subject<(int , int)> _requestSelectSubject = new();
    public Observable<(int, int)> RequestSelect => _requestSelectSubject;
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
        // RequestLoadTextSubjectでも更新されるが、タイトルのIsDirty(*)がちらつくのでここで既に入れておく。
        Document.Text.Value = result.Content;
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
        Document.Reset();
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
    public void Undo()
    {
        _requestUndoSubject.OnNext(Unit.Default);
    }
    public void Redo()
    {
        _requestRedoSubject.OnNext(Unit.Default);
    }
    public bool Find(string target, bool matchCase, bool wrapAround, bool searchUp)
    {
        var doc = Document;
        var text = doc.Text.Value;
        var options = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        int startIndex = searchUp ? doc.CaretIndex.Value : doc.CaretIndex.Value + doc.SelectionLength.Value;
        int foundIndex = -1;

        if (searchUp)
        {
            // 上方向に検索
            foundIndex = text.LastIndexOf(target, startIndex, options);
            if (foundIndex == -1 && wrapAround) // 折り返し
                foundIndex = text.LastIndexOf(target, text.Length, options);
        }
        else
        {
            // 下方向に検索
            foundIndex = text.IndexOf(target, startIndex, options);
            if (foundIndex == -1 && wrapAround) // 折り返し
                foundIndex = text.IndexOf(target, 0, options);
        }

        if (foundIndex != -1)
        {
            // 見つかったらViewへ「選択せよ」と合図を送る
            _requestSelectSubject.OnNext( (foundIndex, target.Length) );
            return true;
        }
        return false;
    }


    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
