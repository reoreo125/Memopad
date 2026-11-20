using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using NSubstitute;
using Reoreo125.Memopad.Views.Behaviors;

namespace Reoreo125.Memopad.Tests.Unit.Views.Behaviors;

public class NumericInputBehaviorTests
{
    /// <summary>
    /// <see cref="KeyEventArgs"/> は非 null の <see cref="PresentationSource"/> を要求するため、
    /// イベント生成用の最小限の実装を用意する。
    /// </summary>
    private sealed class StubPresentationSource : PresentationSource
    {
        private Visual _rootVisual = new DrawingVisual();

        public override Visual RootVisual { get => _rootVisual; set => _rootVisual = value; }
        public override bool IsDisposed => false;
        protected override CompositionTarget? GetCompositionTargetCore() => null;
    }

    private static (TextBox TextBox, NumericInputBehavior Behavior) CreateAttached()
    {
        var textBox = new TextBox();
        var behavior = new NumericInputBehavior();
        Interaction.GetBehaviors(textBox).Add(behavior);
        return (textBox, behavior);
    }

    private static TextCompositionEventArgs RaiseTextInput(TextBox textBox, string text)
    {
        var composition = new TextComposition(InputManager.Current, textBox, text);
        var args = new TextCompositionEventArgs(Keyboard.PrimaryDevice, composition)
        {
            RoutedEvent = TextCompositionManager.PreviewTextInputEvent
        };
        textBox.RaiseEvent(args);
        return args;
    }

    private static KeyEventArgs RaisePreviewKeyDown(TextBox textBox, Key key)
    {
        var args = new KeyEventArgs(Keyboard.PrimaryDevice, new StubPresentationSource(), 0, key)
        {
            RoutedEvent = Keyboard.PreviewKeyDownEvent
        };
        textBox.RaiseEvent(args);
        return args;
    }

    private static DataObjectPastingEventArgs RaisePasting(
        TextBox textBox,
        IDataObject dataObject,
        string? formatToApply = null)
    {
        var args = new DataObjectPastingEventArgs(
            dataObject,
            isDragDrop: false,
            formatToApply ?? DataFormats.Text)
        {
            RoutedEvent = DataObject.PastingEvent
        };
        textBox.RaiseEvent(args);
        return args;
    }

    #region アタッチ・デタッチ
    [StaFact(DisplayName = "【正常系】OnAttached:TextBoxにアタッチされること")]
    public void OnAttached_ShouldAssociateWithTextBox()
    {
        var (textBox, behavior) = CreateAttached();

        Assert.Contains(behavior, Interaction.GetBehaviors(textBox));
        Assert.True(RaiseTextInput(textBox, "a").Handled);
    }

    [StaFact(DisplayName = "【正常系】OnDetaching:デタッチ後は入力・貼り付けを制限しないこと")]
    public void OnDetaching_ShouldRemoveEventHandlers()
    {
        var (textBox, behavior) = CreateAttached();
        Interaction.GetBehaviors(textBox).Remove(behavior);

        Assert.DoesNotContain(behavior, Interaction.GetBehaviors(textBox));
        Assert.False(RaiseTextInput(textBox, "abc").Handled);
        Assert.False(RaisePreviewKeyDown(textBox, Key.Space).Handled);

        var dataObject = Substitute.For<IDataObject>();
        dataObject.GetDataPresent(DataFormats.Text).Returns(true);
        dataObject.GetData(DataFormats.Text).Returns("abc");
        Assert.False(RaisePasting(textBox, dataObject).CommandCancelled);
    }
    #endregion

    #region 文字入力
    [StaTheory(DisplayName = "【正常系】PreviewTextInput:数字のみの入力が許可されること")]
    [InlineData("0")]
    [InlineData("123")]
    [InlineData("007")]
    public void PreviewTextInput_WithDigits_ShouldNotBeHandled(string text)
    {
        var (textBox, _) = CreateAttached();

        var args = RaiseTextInput(textBox, text);

        Assert.False(args.Handled);
    }

    [StaTheory(DisplayName = "【異常系】PreviewTextInput:数字以外を含む入力が拒否されること")]
    [InlineData("a")]
    [InlineData("12a")]
    [InlineData("1.5")]
    [InlineData("-1")]
    [InlineData("１２３")]
    [InlineData("")]
    public void PreviewTextInput_WithNonDigits_ShouldBeHandled(string text)
    {
        var (textBox, _) = CreateAttached();

        var args = RaiseTextInput(textBox, text);

        Assert.True(args.Handled);
    }
    #endregion

    #region キー入力
    [StaFact(DisplayName = "【異常系】PreviewKeyDown:スペースキーが拒否されること")]
    public void PreviewKeyDown_WithSpace_ShouldBeHandled()
    {
        var (textBox, _) = CreateAttached();

        var args = RaisePreviewKeyDown(textBox, Key.Space);

        Assert.True(args.Handled);
    }

    [StaTheory(DisplayName = "【正常系】PreviewKeyDown:スペース以外のキーが許可されること")]
    [InlineData(Key.D0)]
    [InlineData(Key.D9)]
    [InlineData(Key.Back)]
    [InlineData(Key.Enter)]
    public void PreviewKeyDown_WithNonSpaceKey_ShouldNotBeHandled(Key key)
    {
        var (textBox, _) = CreateAttached();

        var args = RaisePreviewKeyDown(textBox, key);

        Assert.False(args.Handled);
    }
    #endregion

    #region 貼り付け
    [StaTheory(DisplayName = "【正常系】Pasting:数字のみのテキストが貼り付けられること")]
    [InlineData("0")]
    [InlineData("123")]
    [InlineData("007")]
    public void Pasting_WithDigits_ShouldNotBeCancelled(string text)
    {
        var (textBox, _) = CreateAttached();
        var dataObject = Substitute.For<IDataObject>();
        dataObject.GetDataPresent(DataFormats.Text).Returns(true);
        dataObject.GetData(DataFormats.Text).Returns(text);

        var args = RaisePasting(textBox, dataObject);

        Assert.False(args.CommandCancelled);
    }

    [StaTheory(DisplayName = "【異常系】Pasting:数字以外を含むテキストが貼り付けられないこと")]
    [InlineData("a")]
    [InlineData("12a")]
    [InlineData("1.5")]
    [InlineData("１２３")]
    [InlineData("")]
    public void Pasting_WithNonDigits_ShouldBeCancelled(string text)
    {
        var (textBox, _) = CreateAttached();
        var dataObject = Substitute.For<IDataObject>();
        dataObject.GetDataPresent(DataFormats.Text).Returns(true);
        dataObject.GetData(DataFormats.Text).Returns(text);

        var args = RaisePasting(textBox, dataObject);

        Assert.True(args.CommandCancelled);
    }

    [StaFact(DisplayName = "【異常系】Pasting:テキスト以外のデータが貼り付けられないこと")]
    public void Pasting_WithNonTextData_ShouldBeCancelled()
    {
        var (textBox, _) = CreateAttached();
        var dataObject = Substitute.For<IDataObject>();
        dataObject.GetDataPresent(DataFormats.Text).Returns(false);
        dataObject.GetDataPresent(DataFormats.Bitmap).Returns(true);

        var args = RaisePasting(textBox, dataObject, DataFormats.Bitmap);

        Assert.True(args.CommandCancelled);
    }
    #endregion
}
