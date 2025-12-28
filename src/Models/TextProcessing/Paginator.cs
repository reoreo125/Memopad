using System;
using System.Collections.Generic;
using System.Globalization;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Reoreo125.Memopad.Models.TextProcessing;

public class Paginator : DocumentPaginator
{
    private readonly DocumentPaginator _basePaginator;
    private readonly PageSettings _settings;
    private readonly string _filePath;

    public Paginator(DocumentPaginator basePaginator, PageSettings settings, string filePath)
    {
        _basePaginator = basePaginator;
        _settings = settings;
        _filePath = filePath;
    }

    public override DocumentPage GetPage(int pageNumber)
    {
        var page = _basePaginator.GetPage(pageNumber);

        var rootVisual = new ContainerVisual();

        rootVisual.Children.Add(page.Visual);

        var hfVisual = new DrawingVisual();
        using (var dc = hfVisual.RenderOpen())
        {
            DrawHeaderFooter(dc, pageNumber + 1);
        }
        rootVisual.Children.Add(hfVisual);

        return new DocumentPage(rootVisual, page.Size, page.BleedBox, page.ContentBox);
    }

    private void DrawHeaderFooter(DrawingContext dc, int pageNum)
    {
        var typeface = new Typeface("Segoe UI"); // または設定フォント
        double fontSize = 10 * 96.0 / 72.0; // 10pt

        // マクロ置換 (&f: ファイル名, &p: ページ番号)
        string headerStr = _settings.Header.Value.Replace("&f", _filePath).Replace("&p", pageNum.ToString());
        string footerStr = _settings.Footer.Value.Replace("&f", _filePath).Replace("&p", pageNum.ToString());

        // ヘッダー（中央上部）
        var headerText = new FormattedText(headerStr, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black, 1.0);
        dc.DrawText(headerText, new Point((PageSize.Width - headerText.Width) / 2, 20));

        // フッター（中央下部）
        var footerText = new FormattedText(footerStr, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black, 1.0);
        dc.DrawText(footerText, new Point((PageSize.Width - footerText.Width) / 2, PageSize.Height - 40));
    }

    public override bool IsPageCountValid => _basePaginator.IsPageCountValid;
    public override int PageCount => _basePaginator.PageCount;
    public override Size PageSize { get => _basePaginator.PageSize; set => _basePaginator.PageSize = value; }
    public override IDocumentPaginatorSource Source => _basePaginator.Source;
}
