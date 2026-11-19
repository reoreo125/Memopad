using System.Reflection;
using Prism.Dialogs;

namespace Reoreo125.Memopad.Tests.TestHelpers;

/// <summary>
/// Prism の <see cref="DialogCloseListener"/> をテスト用に生成するヘルパー。
/// 引数なしの Invoke は内部コールバックが未設定のためプロセスを終了させる。
/// 内部コンストラクターへコールバックを渡し、ViewModel の RequestClose に注入して使用する。
/// </summary>
internal static class DialogCloseListenerTestHelper
{
    private const string RequestCloseBackingFieldName = "<RequestClose>k__BackingField";

    private static readonly ConstructorInfo ListenerConstructor = typeof(DialogCloseListener)
        .GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(Action<IDialogResult>) },
            modifiers: null)
        ?? throw new InvalidOperationException("DialogCloseListener の内部コンストラクターが見つかりません。");

    public static DialogCloseListener Create(Action<IDialogResult> onRequestClose)
        => (DialogCloseListener)ListenerConstructor.Invoke(new object[] { onRequestClose });

    /// <summary>
    /// ViewModel の RequestClose プロパティ（get-only 自動プロパティ）へリスナーを注入する。
    /// </summary>
    public static void AttachRequestClose(object dialogViewModel, Action<IDialogResult> onRequestClose)
    {
        var field = FindBackingField(dialogViewModel.GetType())
            ?? throw new InvalidOperationException(
                $"{dialogViewModel.GetType().Name} に {RequestCloseBackingFieldName} が見つかりません。");

        field.SetValue(dialogViewModel, Create(onRequestClose));
    }

    private static FieldInfo? FindBackingField(Type type)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            var field = current.GetField(
                RequestCloseBackingFieldName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            if (field is not null) return field;
        }

        return null;
    }
}
