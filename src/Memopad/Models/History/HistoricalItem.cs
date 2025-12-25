namespace Reoreo125.Memopad.Models.History;

public record HistoricalItem(
    Guid Id,                         // 履歴の一意識別子
    DateTime Timestamp,              // 操作が行われた時刻

    // --- テキストの差分情報 ---
    int Offset,                      // 変更開始位置（インデックス）
    string OldText,                  // 置き換えられた古い文字列（削除分）
    string NewText,                  // 新しく挿入された文字列（挿入分）

    // --- 操作の種類とメタ情報 ---
    HistoryOperation Operation,      // 操作タイプ（挿入/削除/置換/一括など）
    string Description,              // 「日付挿入」「置換」など履歴一覧に表示する用

    // --- カーソル・選択状態の復元用 (応用) ---
    // 操作直前の状態（Undoした時にここへ戻す）
    int CaretIndexBefore,
    int SelectionLengthBefore,

    // 操作直後の状態（Redoした時にここへ進める）
    int CaretIndexAfter,
    int SelectionLengthAfter,

    // --- 履歴の管理用フラグ ---
    bool IsMerged = false            // 連続した入力が1つにまとめられたか
);
