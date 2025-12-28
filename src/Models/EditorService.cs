using System.Windows;
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
    public Observable<FindEventArgs> RequestFind { get; }
    public Observable<(int foundIndex, int length)> RequestSelect { get; }
    public Observable<Unit> RequestSelectAll { get; }
    public Observable<GoToLineEventArgs> RequestGoToLine { get; }
    public Observable<ReplaceNextEventArgs> RequestReplaceNext { get; }
    public Observable<ReplaceAllEventArgs> RequestReplaceAll { get; }

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
    public bool FindNext();
    public bool FindPrev();
    public void SelectAll();
    public bool GoToLine(int lineIndex);
    public bool ReplaceNext();
    public bool ReplaceAll();
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
    private readonly Subject<FindEventArgs> _requestFindSubject = new();
    public Observable<FindEventArgs> RequestFind => _requestFindSubject;
    private readonly Subject<(int , int)> _requestSelectSubject = new();
    public Observable<(int, int)> RequestSelect => _requestSelectSubject;
    private readonly Subject<Unit> _requestSelectAllSubject = new();
    public Observable<Unit> RequestSelectAll => _requestSelectAllSubject;
    private readonly Subject<GoToLineEventArgs> _requestGoToLineSubject = new();
    public Observable<GoToLineEventArgs> RequestGoToLine => _requestGoToLineSubject;
    private readonly Subject<ReplaceNextEventArgs> _requestReplaceNextSubject = new();
    public Observable<ReplaceNextEventArgs> RequestReplaceNext => _requestReplaceNextSubject;
    private readonly Subject<ReplaceAllEventArgs> _requestReplaceAllSubject = new();
    public Observable<ReplaceAllEventArgs> RequestReplaceAll => _requestReplaceAllSubject;

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
    public bool FindNext()
    {
        FindEventArgs args = new(
            Document.SearchText.Value,
            false,
            Document.MatchCase.Value,
            Document.WrapAround.Value);
        _requestFindSubject.OnNext(args);

        return args.IsSuccess;
    }
    public bool FindPrev()
    {
        FindEventArgs args = new(
            Document.SearchText.Value,
            true,
            Document.MatchCase.Value,
            Document.WrapAround.Value);
        _requestFindSubject.OnNext(args);

        return args.IsSuccess;
    }
    public void SelectAll()
    {
        _requestSelectAllSubject.OnNext(Unit.Default);
    }
    public bool GoToLine(int lineIndex)
    {
        GoToLineEventArgs args = new GoToLineEventArgs(lineIndex);
        _requestGoToLineSubject.OnNext(args);

        return args.IsSuccess;
    }
    public bool ReplaceNext()
    {
        ReplaceNextEventArgs args = new(
            Document.SearchText.Value,
            Document.ReplaceText.Value,
            Document.MatchCase.Value,
            Document.WrapAround.Value);
        _requestReplaceNextSubject.OnNext(args);

        return args.IsSuccess;
    }
    public bool ReplaceAll()
    {
        ReplaceAllEventArgs args = new(
            Document.SearchText.Value,
            Document.ReplaceText.Value,
            Document.MatchCase.Value);
        _requestReplaceAllSubject.OnNext(args);

        return args.IsSuccess;
    }
    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}

public record GoToLineEventArgs(int LineIndex)
{
    public bool IsSuccess { get; set; }
}
public record ReplaceNextEventArgs(string SearchText, string ReplaceText, bool MatchCase, bool WrapAround)
{
    public bool IsSuccess { get; set; }
}
public record ReplaceAllEventArgs(string SearchText, string ReplaceText, bool MatchCase)
{
    public bool IsSuccess { get; set; }
}
public record FindEventArgs(string SearchText, bool SearchUp, bool MatchCase, bool WrapAround)
{
    public bool IsSuccess { get; set; }
}
