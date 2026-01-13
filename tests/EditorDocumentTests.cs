using Reoreo125.Memopad.Models;

namespace Reoreo125.Memopad.Tests;

public class EditorDocumentTests
{
    [Fact(DisplayName = "【正常系】IsDirty:BaseTextとTextを比較して同じならFalse, 違うならTrueを返すこと")]
    public void IsDirty_WhenTextChanged_ShouldBeTrue()
    {
        var doc = new EditorDocument();
        doc.BaseText.Value = "Initial text";
        doc.Text.Value = "Initial text";
        Assert.False(doc.IsDirty.CurrentValue);

        doc.Text.Value = "Modified text";

        Assert.True(doc.IsDirty.CurrentValue);
    }
    
    [Fact(DisplayName = "【正常系】Title:BaseTextとTextを比較して違うならタイトルの先頭に'*'(アスタリスク)があること")]
    public void Title_WhenIsDirtyChanges_ShouldUpdate()
    {
        var doc = new EditorDocument();
        doc.BaseText.Value = "Initial text";
        doc.Text.Value = "Initial text";
        
        string initialTitle = doc.Title.CurrentValue;
        Assert.False(initialTitle.StartsWith('*'));

        doc.Text.Value = "Modified text";

        string dirtyTitle = doc.Title.CurrentValue;
        Assert.True(dirtyTitle.StartsWith('*'));
        
        doc.Text.Value = "Initial text";

        string cleanTitle = doc.Title.CurrentValue;
        Assert.False(cleanTitle.StartsWith('*'));
    }
}
