using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Controls;
using R3;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.Models;

public class EditorDocument
{
    // --- 根幹データ (Primary State) ---
    public ReactiveProperty<string> Text { get; } = new(string.Empty);
    public ReactiveProperty<string> FilePath { get; } = new(string.Empty);

    public ReactiveProperty<Encoding> Encoding { get; } = new(Defaults.Encoding);
    public ReactiveProperty<bool> HasBom { get; } = new(Defaults.HasBOM);
    public ReactiveProperty<LineEnding> LineEnding { get; } = new(Defaults.LineEnding);

    public ReactiveProperty<bool> IsDirty { get; } = new (false);

    // --- 表示用データ (Derived State) ---
    public ReadOnlyReactiveProperty<string> FileName { get; }
    public ReadOnlyReactiveProperty<string> FileNameWithoutExtension { get; }
    public ReadOnlyReactiveProperty<string> Title { get; }


    // --- コンテキスト (Editor State) ---
    public ReactiveProperty<int> CaretIndex { get; } = new(0);
    public ReactiveProperty<int> SelectionLength { get; } = new(0);
    public ReactiveProperty<int> Row { get; } = new(1);
    public ReactiveProperty<int> Column { get; } = new(1);

    public EditorDocument()
    {
        // タイトルを作るためのインナーメソッド
        string CreateTitle() => $"{(IsDirty.Value ? "*" : "")}{FileNameWithoutExtension!.CurrentValue} - {Defaults.ApplicationName}";

        // Derived State の組み立て
        FileName = FilePath.Select(path => string.IsNullOrEmpty(path) ? $"{Defaults.NewFileName}{Defaults.FileExtension}" : Path.GetFileName(path))
                           .ToReadOnlyReactiveProperty($"{Defaults.NewFileName}{Defaults.FileExtension}");
        FileNameWithoutExtension = FilePath.Select(path => string.IsNullOrEmpty(path) ? $"{Defaults.NewFileName}" : Path.GetFileNameWithoutExtension(path))
                                           .ToReadOnlyReactiveProperty($"{Defaults.NewFileName}");
        Title = Observable.Merge(FileNameWithoutExtension!.AsUnitObservable(),
                                 IsDirty.AsUnitObservable())
                          .Select(_ => CreateTitle())
                          .ToReadOnlyReactiveProperty(CreateTitle());
    }
}
