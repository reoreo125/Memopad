using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Commands;

public abstract class CommandBase : ICommand
{
    public virtual event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public abstract bool CanExecute(object? parameter);

    public abstract void Execute(object? parameter);

    /// <summary>
    /// [Dependency] 属性で注入される依存オブジェクトが null（未注入）でないことを検証する。
    /// </summary>
    /// <param name="value">検証する依存オブジェクト。</param>
    /// <param name="name">呼び出し元の引数式（プロパティ名）。省略可能。</param>
    /// <exception cref="InvalidOperationException">依存オブジェクトが未注入の場合。</exception>
    protected static void RequireInjected([NotNull] object? value, [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        if (value is null)
        {
            throw new InvalidOperationException($"{name} が注入されていません。DIコンテナへの登録と [Dependency] 属性の付与を確認してください。");
        }
    }
}
