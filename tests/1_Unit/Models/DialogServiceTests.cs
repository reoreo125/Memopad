using NSubstitute;
using Prism.Dialogs;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Views.Dialogs;
using System;
using IPrismDialogService = Prism.Dialogs.IDialogService;
using DialogService = Reoreo125.Memopad.Models.DialogService;

namespace Reoreo125.Memopad.Tests.Unit.Models;

public class DialogServiceTests
{
    #region ConfirmSave
    [Fact(DisplayName = "【正常系】ConfirmSave: 確認ダイアログが表示されること")]
    public void ConfirmSave_ShouldShowDialog()
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();

        var expectedResult = new DialogResult(ButtonResult.OK);
        prismDialogService.WhenForAnyArgs(x => x.ShowDialog(default, default, default))
            .Do(callInfo =>
            {
                var callback = callInfo[2] as dynamic;
                callback?.Invoke(expectedResult);
            });

        var dialogService = new DialogService
        {
            PrismDialogService = prismDialogService,
        };

        var actualResult = dialogService.ShowConfirmSave("TestText");

        Assert.NotNull(actualResult);
        Assert.Equal(expectedResult, actualResult);
    }
    #endregion
}
