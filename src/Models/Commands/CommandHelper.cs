namespace Reoreo125.Memopad.Models.Commands;

/// <summary>
/// コマンドから利用する共通フローを提供する静的ヘルパー。
/// </summary>
public static class CommandHelper
{
    /// <summary>
    /// 未保存の変更がある場合に保存確認ダイアログを表示し、保存先が未設定なら「名前を付けて保存」ダイアログで保存先を決定する。
    /// 実際のファイル書き込みは行わないため、保存が必要な場合は呼び出し側で EditorService.SaveText を実行すること。
    /// </summary>
    /// <param name="editorService">確認対象のドキュメントを保持するサービス。</param>
    /// <param name="dialogService">確認ダイアログを表示するサービス。</param>
    /// <param name="saveRequested">ユーザーが保存を選択した場合は true。</param>
    /// <returns>後続の処理を続行できる場合は true。ダイアログでキャンセルされた場合は false。</returns>
    public static bool TryConfirmSave(IEditorService editorService, IDialogService dialogService, out bool saveRequested)
    {
        saveRequested = false;

        if (editorService.Document.IsDirty.CurrentValue)
        {
            IDialogResult? result = dialogService.ShowConfirmSave(editorService.Document.FileNameWithoutExtension.CurrentValue);
            if (result is null) return false;

            if (result.Result is ButtonResult.Yes)
            {
                saveRequested = true;
            }
            else if (result.Result is ButtonResult.No) { }
            else
            {
                return false;
            }
        }

        if (saveRequested && string.IsNullOrEmpty(editorService.Document.FilePath.Value))
        {
            var saveFiledialogResult = dialogService.ShowSaveFile();

            if (saveFiledialogResult is null) return false;
            if (saveFiledialogResult.Result is not ButtonResult.OK) return false;

            editorService.Document.FilePath.Value = saveFiledialogResult.Parameters.GetValue<string>("filename");
        }

        return true;
    }
}
