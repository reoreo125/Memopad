using System.Reflection;
using System.Text;
using NSubstitute;
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

    #region LoadText
    [Fact(DisplayName = "【正常系】LoadText:テキストファイルが読み出され、RequestLoadTextが呼び出されること")]
    public void LoadText_WhenLoadValidTextFile_ShouldNotifyRequestLoadText()
    {
        var textFileService = Substitute.For<ITextFileService>();
        var settingsService = new SettingsService();
        var dialogService = Substitute.For<IDialogService>();
        var editorService = new EditorService(textFileService, settingsService, dialogService);

        var loadResult = new TextFileLoadResult(true, @"c:\DummyPath.txt", "TestContent", Encoding.UTF8, false, LineEnding.CRLF);
        textFileService.Load(loadResult.FilePath).Returns(loadResult);

        string notifiedContent = string.Empty;
        using (editorService.RequestLoadText.Subscribe(loadText => notifiedContent = loadText))
        {
            editorService.LoadText(loadResult.FilePath);
        }

        Assert.Equal("TestContent", notifiedContent);
        Assert.Equal("TestContent", editorService.Document.BaseText.Value);
        Assert.Equal("TestContent", editorService.Document.Text.Value);
        Assert.Equal(@"c:\DummyPath.txt", editorService.Document.FilePath.Value);
        Assert.Equal(@"c:\", settingsService.Settings.LastOpenedFolderPath.Value);
    }
    [Fact(DisplayName = "【異常系】LoadText:テキストファイルの読み込みに失敗し、RequestLoadTextが呼び出さず、DialogServiceのShowFileLoadErrorが呼び出される")]
    public void LoadText_LoadFailed()
    {
        var textFileService = Substitute.For<ITextFileService>();
        var settingsService = new SettingsService();
        var dialogService = Substitute.For<IDialogService>();
        var editorService = new EditorService(textFileService, settingsService, dialogService);
        editorService.Document.Text.Value = "Old Content";
        var loadResult = new TextFileLoadResult(false, @"c:\DummyPath.txt", string.Empty, Defaults.Encoding, Defaults.HasBOM, Defaults.LineEnding);
        textFileService.Load(loadResult.FilePath).Returns(loadResult);

        bool notificationResult = false;
        using (editorService.RequestLoadText.Subscribe(loadText => notificationResult = true))
        {
            editorService.LoadText(loadResult.FilePath);
        }

        Assert.False(notificationResult);
        dialogService.Received(1).ShowFileLoadError();
        Assert.Equal("Old Content", editorService.Document.Text.Value);
    }
    #endregion
}
