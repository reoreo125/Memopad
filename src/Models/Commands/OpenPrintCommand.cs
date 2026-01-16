using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Reoreo125.Memopad.Models.TextProcessing;

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

        var printdialogResult = DialogService.ShowPrint();

        if (printdialogResult is null) return;
        if (printdialogResult.Result is not ButtonResult.OK) return;

        var dialog = printdialogResult.Parameters.GetValue<PrintDialog>("printdialog");
        FlowDocument doc = new FlowDocument();

        double mmToDpi = 96.0 / 25.4;
        doc.PagePadding = new Thickness(
            SettingsService.Settings.Page.MarginLeft.Value * mmToDpi,
            SettingsService.Settings.Page.MarginTop.Value * mmToDpi,
            SettingsService.Settings.Page.MarginRight.Value * mmToDpi,
            SettingsService.Settings.Page.MarginBottom.Value * mmToDpi
        );

        doc.PageWidth = dialog.PrintTicket.PageMediaSize.Width ?? 816;
        doc.PageHeight = dialog.PrintTicket.PageMediaSize.Height ?? 1056;
        doc.ColumnGap = 0;
        doc.ColumnWidth = doc.PageWidth - doc.PagePadding.Left - doc.PagePadding.Right;

        var paragraph = new Paragraph(new Run(EditorService.Document.Text.Value));
        paragraph.FontFamily = new FontFamily(SettingsService.Settings.FontFamilyName.Value);
        paragraph.FontSize = SettingsService.Settings.FontSize.Value;
        doc.Blocks.Add(paragraph);

        var wrapper = new Paginator(
            ((IDocumentPaginatorSource)doc).DocumentPaginator,
            SettingsService.Settings.Page,
            EditorService.Document.FilePath.Value
        );

        dialog.PrintDocument(wrapper, "Memopad");
        
    }
}
