namespace Reoreo125.Memopad.Models.History;

public enum HistoryOperation
{
    Default,        // 未定義
    Insert,         // 文字入力、貼り付け
    Delete,         // Backspace, Delete
    Replace,        // 置換、日付挿入、選択範囲への上書き
    FormatChange,   // 改行コードの一括変換など（テキスト全体が対象）
}
