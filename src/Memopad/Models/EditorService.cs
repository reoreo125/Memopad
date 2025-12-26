using System.IO;
using System.Text;
using System.Windows;
using R3;
using Reoreo125.Memopad.Models.History;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.Models;

public interface IEditorService : IDisposable
{
    public EditorDocument Document { get; }
    public Settings Settings { get; }

    public void Reset();
    public void NofityAllChanges();
    public void LoadText(string filePath);
    public void SaveText();
    public void SaveText(string filePath);

    public Observable<Unit> RequestCut { get; }
    public Observable<Unit> RequestCopy { get; }
    public Observable<Unit> RequestPaste { get; }
    public Observable<Unit> RequestDelete { get; }
    public Observable<string> RequestInsert { get; }

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


    public bool CanCheckDirty { get; set; } = true;

    public EditorDocument Document { get; }
    public Settings Settings => MemopadSettingsService.Settings;
    private ISettingsService MemopadSettingsService { get; }
    private ITextFileService? TextFileService { get; }
    private IHistoricalService? HistoricalService { get; }

    private DisposableBag _disposableCollection = new();

    public EditorService(ISettingsService settingsService,
                         ITextFileService textFileService,
                         IHistoricalService historicalService)
    {
        MemopadSettingsService = settingsService;
        TextFileService = textFileService;
        HistoricalService = historicalService;

        Document = new EditorDocument();

        Reset();

        // テキスト内容が変化したら変更フラグを立てる
        Document.Text
            .Pairwise()
            .Subscribe(pair =>
            {
                if (CanCheckDirty && !Document.IsDirty.Value && pair.Previous != pair.Current)
                {
                    Document.IsDirty.Value = true;
                }
            })
            .AddTo(ref _disposableCollection);
    }

    private void EnableCheckDirty() => CanCheckDirty = true;
    private void DisableCheckDirty() => CanCheckDirty = false;

    public void Reset()
    {
        Document.Text.Value = string.Empty;
        Document.FilePath.Value = string.Empty;

        Document.Encoding.Value = Defaults.Encoding;
        Document.HasBom.Value = Defaults.HasBOM;
        Document.LineEnding.Value = Defaults.LineEnding;
        Document.IsDirty.Value = false;

        Document.CaretIndex.Value = 0;
        Document.SelectionLength.Value = 0;
        Document.Row.Value = 1;
        Document.Column.Value = 1;

        NofityAllChanges();
    }
    public void NofityAllChanges()
    {
        Document.FilePath.ForceNotify();
        Document.Text.ForceNotify();
        Document.IsDirty.ForceNotify();
        Document.Row.ForceNotify();
        Document.Column.ForceNotify();
        Document.Encoding.ForceNotify();
        Document.HasBom.ForceNotify();
        Document.LineEnding.ForceNotify();
        Document.IsDirty.ForceNotify();

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

        DisableCheckDirty();

        Reset();
        Document.Text.Value = result.Content;
        Document.Encoding.Value = result.Encoding;
        Document.HasBom.Value = result.HasBOM;
        // LineEnding が不明な場合はデフォルト値を使う
        Document.LineEnding.Value = (result.LineEnding is LineEnding.Unknown) ? Defaults.LineEnding : result.LineEnding;
        Document.FilePath.Value = result.FilePath;

        EnableCheckDirty();
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

        DisableCheckDirty();

        Document.FilePath.Value = result.FilePath;
        Document.IsDirty.Value = false;

        EnableCheckDirty();

        NofityAllChanges();
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
        /*
        var currentText = Document.Text.Value ?? "";
        var start = Document.CaretIndex.Value;
        var length = Document.SelectionLength.Value;

        // 文字列挿入
        Document.Text.Value = currentText.Remove(start, length).Insert(start, text);

        // カーソル位置を挿入した文字の直後に移動させる
        Document.CaretIndex.Value = start + text.Length;

        // 選択範囲は解除されるので0にする
        Document.SelectionLength.Value = 0;
        */
        _requestInsertSubject.OnNext(text);
    }
    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
