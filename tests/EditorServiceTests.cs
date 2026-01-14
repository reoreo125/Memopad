using System.Reflection;
using System.Text;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.TextProcessing;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests;

public class EditorServiceTests
{
    #region Reset
    [Fact(DisplayName = "【正常系】Reset:ステートが適切にリセットされ、RequestResetが呼び出されること")]
    public void Reset_ShouldNotifyRequestReset()
    {
        var textFileServiceMock = NSubstitute.Substitute.For<ITextFileService>();
        var settingsServiceMock = NSubstitute.Substitute.For<ISettingsService>();
        var dialogServiceMock = NSubstitute.Substitute.For<IDialogService>();
        var editorService = new EditorService(textFileServiceMock, settingsServiceMock, dialogServiceMock);
        editorService.Document.Text.Value = "Modified Text";
        editorService.Document.FilePath.Value = @"c:\Test.txt";
        editorService.Document.Encoding.Value = Encoding.UTF32;

        bool result = false;
        using (editorService.RequestReset.Subscribe(_ => result = true))
        {
            editorService.Reset();
        }

        Assert.True(result);
        Assert.Empty(editorService.Document.Text.Value);
        Assert.Empty(editorService.Document.FilePath.Value);
        Assert.Equal(editorService.Document.Encoding.Value, Defaults.Encoding);
        Assert.False(editorService.Document.CanUndo.Value);
        Assert.False(editorService.Document.CanRedo.Value);
    }
    #endregion
}
