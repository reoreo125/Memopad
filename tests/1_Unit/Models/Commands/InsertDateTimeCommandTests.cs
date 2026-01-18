using NSubstitute;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using System;
using System.Text.RegularExpressions;
using Xunit;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class InsertDateTimeCommandTests
{
    IEditorService EditorService { get; set; }

    public InsertDateTimeCommandTests()
    {
        EditorService = Substitute.For<IEditorService>();
    }

    [Fact(DisplayName = "【正常系】CanExecute: 常にtrueを返すこと")]
    public void CanExecute_AlwaysReturnsTrue()
    {
        var command = new InsertDateTimeCommand { EditorService = EditorService };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: EditorService.Insertが正しい日時フォーマットで呼ばれること")]
    public void Execute_ShouldCallEditorServiceInsertWithCorrectFormat()
    {
        var command = new InsertDateTimeCommand { EditorService = EditorService };
        string? capturedDateTime = null;

        EditorService.Insert(Arg.Do<string>(x => capturedDateTime = x));

        command.Execute(null);

        EditorService.Received(1).Insert(Arg.Any<string>());
        Assert.NotNull(capturedDateTime);

        // "H:mm yyyy/MM/dd" フォーマットの正規表現
        // 例: "15:30 2023/10/27"
        var regex = new Regex(@"^\d{1,2}:\d{2} \d{4}\/\d{2}\/\d{2}$");
        Assert.Matches(regex, capturedDateTime);

        // さらに厳密にするなら、ParseExactなどで実際に日付として解析できるかも確認できる
        // ただし、秒やミリ秒が含まれないため、厳密な DateTime.Now との比較は困難
        // 例外が発生しないことを確認する
        DateTime parsedDateTime;
        Assert.True(DateTime.TryParseExact(capturedDateTime, "H:mm yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDateTime));
    }
}
