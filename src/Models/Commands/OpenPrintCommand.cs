using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Reoreo125.Memopad.Models.Commands;

public interface IOpenPrintCommand : ICommand {}

public class OpenPrintCommand : CommandBase, IOpenPrintCommand
{
    [Dependency]
    public IDialogService? DialogService { get; set; }
    [Dependency]
    public IEditorService? EditorService { get; set; }
    [Dependency]
    public ISettingsService? SettingsService { get; set; }

    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        if (DialogService is null) throw new Exception(nameof(DialogService));
        if (EditorService is null) throw new Exception(nameof(EditorService));
        if (SettingsService is null) throw new Exception(nameof(SettingsService));

        (bool? result, PrintDialog dialog) = DialogService.ShowPrint();

        if(result is true)
        {
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.ColumnGap = 0;
            doc.ColumnWidth = dialog.PrintableAreaWidth;

            var paragraph = new Paragraph();
            var run = new Run(EditorService.Document.Text.Value);
            paragraph.Inlines.Add(run);

            paragraph.FontFamily = new FontFamily(SettingsService.Settings.FontFamilyName.Value);
            paragraph.FontSize = SettingsService.Settings.FontSize.Value;

            doc.Blocks.Add(paragraph);

            IDocumentPaginatorSource idpSource = doc;
            dialog.PrintDocument(idpSource.DocumentPaginator, "Memopad");
        }
    }
}
