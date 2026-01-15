using Reoreo125.Memopad.Models;
using System.IO;

namespace Reoreo125.Memopad.Tests.Unit.Models;

public class EditorDocumentTests
{
    #region Constructor
    [Fact(DisplayName = "【正常系】コンストラクタ:各プロパティが初期値で生成されること")]
    public void Constructor_InitializesProperties()
    {
        var doc = new EditorDocument();

        Assert.Equal(string.Empty, doc.Text.Value);
        Assert.Equal(string.Empty, doc.BaseText.Value);
        Assert.Equal(string.Empty, doc.FilePath.Value);
        Assert.Equal(Defaults.Encoding, doc.Encoding.Value);
        Assert.Equal(Defaults.HasBOM, doc.HasBom.Value);
        Assert.Equal(Defaults.LineEnding, doc.LineEnding.Value);
        Assert.False(doc.IsDirty.CurrentValue);
        Assert.Equal($"{Defaults.NewFileName}{Defaults.FileExtension}", doc.FileName.CurrentValue);
        Assert.Equal(Defaults.NewFileName, doc.FileNameWithoutExtension.CurrentValue);
        Assert.Equal($"{Defaults.NewFileName} - {Defaults.ApplicationName}", doc.Title.CurrentValue);
    }
    #endregion

    #region Reset
    [Fact(DisplayName = "【正常系】Reset:各プロパティが初期値にリセットされること")]
    public void Reset_ResetsProperties()
    {
        var doc = new EditorDocument();
        // Modify some properties
        doc.Text.Value = "some text";
        doc.BaseText.Value = "some base text";
        doc.FilePath.Value = "C:\\test.txt";
        doc.Encoding.Value = System.Text.Encoding.ASCII;
        doc.HasBom.Value = !Defaults.HasBOM;
        doc.LineEnding.Value = Models.TextProcessing.LineEnding.CRLF;

        doc.Reset();

        Assert.Equal(string.Empty, doc.Text.Value);
        Assert.Equal(string.Empty, doc.BaseText.Value);
        Assert.Equal(string.Empty, doc.FilePath.Value);
        Assert.Equal(Defaults.Encoding, doc.Encoding.Value);
        Assert.Equal(Defaults.HasBOM, doc.HasBom.Value);
        Assert.Equal(Defaults.LineEnding, doc.LineEnding.Value);
        Assert.False(doc.IsDirty.CurrentValue);
        Assert.Equal($"{Defaults.NewFileName}{Defaults.FileExtension}", doc.FileName.CurrentValue);
        Assert.Equal(Defaults.NewFileName, doc.FileNameWithoutExtension.CurrentValue);
        Assert.Equal($"{Defaults.NewFileName} - {Defaults.ApplicationName}", doc.Title.CurrentValue);
    }
    #endregion

    #region FileName
    [Fact(DisplayName = "【正常系】FileName:FilePathが変更されたらFileNameも変更されること")]
    public void FileName_WhenFilePathChanges_ShouldUpdate()
    {
        var doc = new EditorDocument();
        var filePath = "C:\\test\\test.txt";
        doc.FilePath.Value = filePath;

        Assert.Equal(Path.GetFileName(filePath), doc.FileName.CurrentValue);
    }
    #endregion

    #region FileNameWithoutExtension
    [Fact(DisplayName = "【正常系】FileNameWithoutExtension:FilePathが変更されたらFileNameWithoutExtensionも変更されること")]
    public void FileNameWithoutExtension_WhenFilePathChanges_ShouldUpdate()
    {
        var doc = new EditorDocument();
        var filePath = "C:\\test\\test.txt";
        doc.FilePath.Value = filePath;

        Assert.Equal(Path.GetFileNameWithoutExtension(filePath), doc.FileNameWithoutExtension.CurrentValue);
    }
    #endregion

    #region IsDirty
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
    #endregion

    #region Title
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

    [Fact(DisplayName = "【正常系】Title:FilePathが変更されたらTitleも変更されること")]
    public void Title_WhenFilePathChanges_ShouldUpdate()
    {
        var doc = new EditorDocument();
        var filePath = "C:\\test\\newfile.txt";
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        
        doc.FilePath.Value = filePath;

        var expectedTitle = $"{fileNameWithoutExtension} - {Defaults.ApplicationName}";
        Assert.Equal(expectedTitle, doc.Title.CurrentValue);
    }
    #endregion
}
