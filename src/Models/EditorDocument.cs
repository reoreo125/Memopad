using System.IO;
using System.Text;
using R3;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.Models;

public interface IEditorDocument
{
    public ReactiveProperty<string> Text { get; }
    public ReactiveProperty<string> BaseText { get; }
    public ReactiveProperty<string> FilePath { get; }

    public ReactiveProperty<Encoding> Encoding { get; }
    public ReactiveProperty<bool> HasBom { get; }
    public ReactiveProperty<LineEnding> LineEnding { get; }

    // --- 表示用データ (Derived State) ---
    public ReadOnlyReactiveProperty<string> FileName { get; }
    public ReadOnlyReactiveProperty<string> FileNameWithoutExtension { get; }
    public ReadOnlyReactiveProperty<string> Title { get; }
    public ReadOnlyReactiveProperty<bool> IsDirty { get; }

    // --- コンテキスト (Editor State) ---
    public ReactiveProperty<bool> CanUndo { get; }
    public ReactiveProperty<bool> CanRedo { get; }
    public ReactiveProperty<int> CaretIndex { get; }
    public ReactiveProperty<string> SelectedText { get; }
    public ReactiveProperty<int> SelectionLength { get; }
    public ReactiveProperty<int> Row { get; }
    public ReactiveProperty<int> Column { get; }

    public ReactiveProperty<string> SearchText { get; }
    public ReactiveProperty<string> ReplaceText { get; }
    public ReactiveProperty<bool> MatchCase { get; }
    public ReactiveProperty<bool> WrapAround { get; }

    public void Reset();
}
public class EditorDocument : IEditorDocument, IDisposable
{
    // --- 根幹データ (Primary State) ---
    public ReactiveProperty<string> Text { get; } = new(string.Empty);
    public ReactiveProperty<string> BaseText { get; } = new(string.Empty);
    public ReactiveProperty<string> FilePath { get; } = new(string.Empty);

    public ReactiveProperty<Encoding> Encoding { get; } = new(Defaults.Encoding);
    public ReactiveProperty<bool> HasBom { get; } = new(Defaults.HasBOM);
    public ReactiveProperty<LineEnding> LineEnding { get; } = new(Defaults.LineEnding);

    // --- 表示用データ (Derived State) ---
    public ReadOnlyReactiveProperty<string> FileName { get; }
    public ReadOnlyReactiveProperty<string> FileNameWithoutExtension { get; }
    public ReadOnlyReactiveProperty<string> Title { get; }
    public ReadOnlyReactiveProperty<bool> IsDirty { get; }

    // --- コンテキスト (Editor State) ---
    public ReactiveProperty<bool> CanUndo { get; } = new(false);
    public ReactiveProperty<bool> CanRedo { get; } = new(false);
    public ReactiveProperty<int> CaretIndex { get; } = new(0);
    public ReactiveProperty<string> SelectedText { get; } = new(string.Empty);
    public ReactiveProperty<int> SelectionLength { get; } = new(0);
    public ReactiveProperty<int> Row { get; } = new(1);
    public ReactiveProperty<int> Column { get; } = new(1);

    public ReactiveProperty<string> SearchText { get; } = new(string.Empty);
    public ReactiveProperty<string> ReplaceText { get; } = new(string.Empty);
    public ReactiveProperty<bool> MatchCase { get; } = new(Defaults.MatchCase);
    public ReactiveProperty<bool> WrapAround { get; } = new(Defaults.WrapAround);


    private DisposableBag _disposableCollection = new();

    public EditorDocument()
    {
        IsDirty = Text.CombineLatest(BaseText, (current, baseText) => current != baseText)
              .ToReadOnlyReactiveProperty(false);

        // タイトルを作るためのインナーメソッド
        string CreateTitle() => $"{(IsDirty!.CurrentValue ? "*" : "")}{FileNameWithoutExtension!.CurrentValue} - {Defaults.ApplicationName}";

        // Derived State の組み立て
        FileName = FilePath.Select(path => string.IsNullOrEmpty(path) ? $"{Defaults.NewFileName}{Defaults.FileExtension}" : Path.GetFileName(path))
                           .ToReadOnlyReactiveProperty($"{Defaults.NewFileName}{Defaults.FileExtension}");
        FileNameWithoutExtension = FilePath.Select(path => string.IsNullOrEmpty(path) ? $"{Defaults.NewFileName}" : Path.GetFileNameWithoutExtension(path))
                                           .ToReadOnlyReactiveProperty($"{Defaults.NewFileName}");
        Title = Observable.Merge(FileNameWithoutExtension!.AsUnitObservable(),
                                 IsDirty!.AsUnitObservable())
                          .Select(_ => CreateTitle())
                          .ToReadOnlyReactiveProperty(CreateTitle());
    }
    public void Reset()
    {
        Text.Value = string.Empty;
        BaseText.Value = string.Empty;
        FilePath.Value = string.Empty;

        Encoding.Value = Defaults.Encoding;
        HasBom.Value = Defaults.HasBOM;
        LineEnding.Value = Defaults.LineEnding;

        CanUndo.Value = false;
        CanRedo.Value = false;
        CaretIndex.Value = 0;
        SelectedText.Value = string.Empty;
        SelectionLength.Value = 0;
        Row.Value = 1;
        Column.Value = 1;

        SearchText.Value = string.Empty;
        ReplaceText.Value = string.Empty;
        MatchCase.Value = Defaults.MatchCase;
        WrapAround.Value = Defaults.WrapAround;
    }
    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
