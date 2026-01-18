using NSubstitute;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using Xunit;

namespace Reoreo125.Memopad.Tests.Unit.Models.Commands;

public class InsertCommandTests
{
    IEditorService EditorService { get; set; }

    public InsertCommandTests()
    {
        EditorService = Substitute.For<IEditorService>();
    }

    [Fact(DisplayName = "【正常系】CanExecute: 常にtrueを返すこと")]
    public void CanExecute_AlwaysReturnsTrue()
    {
        var command = new InsertCommand { EditorService = EditorService };

        var canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact(DisplayName = "【正常系】Execute: 文字列パラメータが渡された場合、EditorService.Insertが呼ばれること")]
    public void Execute_ParameterIsString_ShouldCallEditorServiceInsert()
    {
        string textToInsert = "test text";
        var command = new InsertCommand { EditorService = EditorService };

        command.Execute(textToInsert);

        EditorService.Received(1).Insert(textToInsert);
    }

    [Fact(DisplayName = "【正常系】Execute: 文字列以外のパラメータが渡された場合、EditorService.Insertは呼ばれないこと")]
    public void Execute_ParameterIsNotString_ShouldNotCallEditorServiceInsert()
    {
        object nonStringParameter = 123; // stringではないパラメータ
        var command = new InsertCommand { EditorService = EditorService };

        command.Execute(nonStringParameter);

        EditorService.DidNotReceive().Insert(Arg.Any<string>());
    }

    [Fact(DisplayName = "【正常系】Execute: nullパラメータが渡された場合、EditorService.Insertは呼ばれないこと")]
    public void Execute_ParameterIsNull_ShouldNotCallEditorServiceInsert()
    {
        var command = new InsertCommand { EditorService = EditorService };

        command.Execute(null);

        EditorService.DidNotReceive().Insert(Arg.Any<string>());
    }
}
