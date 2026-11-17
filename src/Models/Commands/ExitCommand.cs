using System.Windows;
using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public interface IExitCommand : ICommand {}

public class ExitCommand : CommandBase, IExitCommand
{
    // Application.Shutdown() の終了処理はディスパッチャへポストされ、その後ウィンドウのClosingが再発火する。
    // 再入時のExecute再実行で確認ダイアログが二重に表示されるのを防ぐため、Shutdownに進めたことをマークして以降を抑止する。
    // Shutdownは取り消せずアプリケーションは必ず終了するため、一度trueになったら戻す必要はない。
    private bool _shutdownRequested;

    [Dependency]
    public IEditorService? EditorService { get; set; }
    [Dependency]
    public IDialogService? DialogService { get; set; }
    [Dependency]
    public ISaveAsTextFileCommand? SaveAsTextFileCommand { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        RequireInjected(EditorService);
        RequireInjected(DialogService);
        RequireInjected(SaveAsTextFileCommand);

        if (_shutdownRequested) return;

        if (CommandHelper.TryConfirmSave(EditorService, DialogService, out var save) is false) return;

        // 保存に失敗した場合は終了せずに処理を中断する（編集中の内容を保持する）
        if (save && EditorService.SaveText(EditorService.Document.FilePath.Value) is false) return;

        _shutdownRequested = true;
        Application.Current.Shutdown();
    }
}
