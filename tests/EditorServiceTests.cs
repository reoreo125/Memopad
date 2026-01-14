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
        var textFileServiceMock = Substitute.For<ITextFileService>();
        var settingsServiceMock = Substitute.For<ISettingsService>();
        var dialogServiceMock = Substitute.For<IDialogService>();
        var editorService = new EditorService(textFileServiceMock, settingsServiceMock, dialogServiceMock);
        editorService.Document.Text.Value = "Modified Text";
        editorService.Document.FilePath.Value = @"c:\Test.txt";
        editorService.Document.Encoding.Value = Encoding.UTF32;
        editorService.Document.CanUndo.Value = true;
        editorService.Document.CanRedo.Value = true;

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

        editorService.Document.CanUndo.Value = true;
        editorService.Document.CanRedo.Value = true;

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
        Assert.False(editorService.Document.CanUndo.Value);
        Assert.False(editorService.Document.CanRedo.Value);
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

    #region SaveText
    [Fact(DisplayName = "【正常系】SaveText:テキストファイルが書き出された後、特定のプロパティが正しく設定されること、")]
    public void SaveText_WhenSaveText_ShouldUpdateProperty()
    {
        var textFileService = Substitute.For<ITextFileService>();
        var settingsService = new SettingsService();
        var dialogService = Substitute.For<IDialogService>();
        var editorService = new EditorService(textFileService, settingsService, dialogService);
        editorService.Document.FilePath.Value = @"C:\Users\Public\Documents\DummyPath.txt";
        editorService.Document.Text.Value = "Test Content";
        editorService.Document.BaseText.Value = "Old Content";

        var saveResult = new TextFileSaveResult(true, @"c:\DummyPath.txt");
        textFileService.Save(saveResult.FilePath, "Test Content", Defaults.Encoding, Defaults.HasBOM, Defaults.LineEnding).Returns(saveResult);

        editorService.SaveText(saveResult.FilePath);

        Assert.Equal("Test Content", editorService.Document.BaseText.Value);
        Assert.Equal(@"c:\DummyPath.txt", editorService.Document.FilePath.Value);
        Assert.False(editorService.Document.IsDirty.CurrentValue);
    }
    [Fact(DisplayName = "【異常系】SaveText:テキストファイルの書き込みに失敗し、DialogServiceのShowFileSaveErrorが呼び出される")]
    public void SaveText_SaveFailed()
    {
        var textFileService = Substitute.For<ITextFileService>();
        var settingsService = new SettingsService();
        var dialogService = Substitute.For<IDialogService>();
        var editorService = new EditorService(textFileService, settingsService, dialogService);
        editorService.Document.Text.Value = "Test Content";
        var saveResult = new TextFileSaveResult(false, @"c:\DummyPath.txt");
        textFileService.Save(saveResult.FilePath, "Test Content", Defaults.Encoding, Defaults.HasBOM, Defaults.LineEnding).Returns(saveResult);

        editorService.SaveText(saveResult.FilePath);

        dialogService.Received(1).ShowFileSaveError();
    }
    #endregion
}
