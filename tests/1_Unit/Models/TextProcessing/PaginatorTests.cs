using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.Tests.Unit.Models.TextProcessing;

public class PaginatorTests
{
    private sealed class StubDocumentPaginator : DocumentPaginator
    {
        private readonly DocumentPage _page;
        private readonly IDocumentPaginatorSource _source;
        private Size _pageSize;

        public StubDocumentPaginator(DocumentPage page, Size pageSize, IDocumentPaginatorSource source)
        {
            _page = page;
            _pageSize = pageSize;
            _source = source;
        }

        public int? LastRequestedPageNumber { get; private set; }
        public bool IsPageCountValidValue { get; set; } = true;
        public int PageCountValue { get; set; } = 3;

        public override DocumentPage GetPage(int pageNumber)
        {
            LastRequestedPageNumber = pageNumber;
            return _page;
        }

        public override bool IsPageCountValid => IsPageCountValidValue;
        public override int PageCount => PageCountValue;
        public override Size PageSize { get => _pageSize; set => _pageSize = value; }
        public override IDocumentPaginatorSource Source => _source;
    }

    private sealed class PaginatorFixture
    {
        public DrawingVisual BasePageVisual { get; } = new();
        public Size PageSize { get; } = new(800, 600);
        public Rect BleedBox { get; } = new(0, 0, 800, 600);
        public Rect ContentBox { get; } = new(20, 20, 760, 560);
        public StubDocumentPaginator BasePaginator { get; }
        public Paginator Paginator { get; }

        public PaginatorFixture(
            string header = "Header",
            string footer = "Footer",
            string filePath = "document.txt")
        {
            var page = new DocumentPage(BasePageVisual, PageSize, BleedBox, ContentBox);
            BasePaginator = new StubDocumentPaginator(
                page,
                PageSize,
                Substitute.For<IDocumentPaginatorSource>());

            var settings = Substitute.For<IPageSettings>();
            settings.Header.Returns(new ReactiveProperty<string>(header));
            settings.Footer.Returns(new ReactiveProperty<string>(footer));

            Paginator = new Paginator(BasePaginator, settings, filePath);
        }
    }

    private static List<string> GetDrawTexts(Visual visual)
    {
        var texts = new List<string>();
        if (visual is not DrawingVisual drawingVisual) return texts;
        if (drawingVisual.Drawing is not DrawingGroup rootGroup) return texts;

        // DrawText 呼び出しごとに DrawingGroup が作られるため、
        // 直下の子単位で文字を連結して1つの描画文字列に復元する。
        foreach (var child in rootGroup.Children)
        {
            var parts = new List<string>();
            CollectTexts(child, parts);
            var text = string.Concat(parts);
            if (text.Length > 0) texts.Add(text);
        }

        return texts;
    }

    private static void CollectTexts(Drawing? drawing, List<string> texts)
    {
        switch (drawing)
        {
            case DrawingGroup group:
                foreach (var child in group.Children)
                {
                    CollectTexts(child, texts);
                }
                break;
            case GlyphRunDrawing glyphRunDrawing:
                texts.Add(new string(glyphRunDrawing.GlyphRun.Characters.ToArray()));
                break;
        }
    }

    #region GetPage
    [StaFact(DisplayName = "【正常系】GetPage:ベースのページャーにページ番号が委譲されること")]
    public void GetPage_ShouldDelegatePageNumber()
    {
        var fixture = new PaginatorFixture();

        _ = fixture.Paginator.GetPage(4);

        Assert.Equal(4, fixture.BasePaginator.LastRequestedPageNumber);
    }

    [StaFact(DisplayName = "【正常系】GetPage:ページサイズ・裁ち落とし・内容領域が維持されること")]
    public void GetPage_ShouldPreservePageMetrics()
    {
        var fixture = new PaginatorFixture();

        var result = fixture.Paginator.GetPage(0);

        Assert.Equal(fixture.PageSize, result.Size);
        Assert.Equal(fixture.BleedBox, result.BleedBox);
        Assert.Equal(fixture.ContentBox, result.ContentBox);
    }

    [StaFact(DisplayName = "【正常系】GetPage:ベースページとヘッダー・フッターの2つのVisualが含まれること")]
    public void GetPage_ShouldContainBasePageAndHeaderFooterVisuals()
    {
        var fixture = new PaginatorFixture();

        var result = fixture.Paginator.GetPage(0);

        var rootVisual = Assert.IsType<ContainerVisual>(result.Visual);
        Assert.Equal(2, rootVisual.Children.Count);
        Assert.Same(fixture.BasePageVisual, rootVisual.Children[0]);
        Assert.IsType<DrawingVisual>(rootVisual.Children[1]);
    }
    #endregion

    #region ヘッダー・フッター
    [StaFact(DisplayName = "【正常系】ヘッダー・フッター:&fと&pがファイルパス・ページ番号に置換されること")]
    public void GetPage_ShouldReplaceFileNameAndPageNumberMacros()
    {
        var fixture = new PaginatorFixture(
            header: "&f - &p",
            footer: "&p / &p",
            filePath: @"C:\docs\report.txt");

        var result = fixture.Paginator.GetPage(1);
        var texts = GetDrawTexts(Assert.IsType<DrawingVisual>(
            Assert.IsType<ContainerVisual>(result.Visual).Children[1]));

        Assert.Contains(@"C:\docs\report.txt - 2", texts);
        Assert.Contains("2 / 2", texts);
    }

    [StaFact(DisplayName = "【正常系】ヘッダー・フッター:先頭ページでは&pが1に置換されること")]
    public void GetPage_OnFirstPage_ShouldReplacePageNumberWithOne()
    {
        var fixture = new PaginatorFixture(header: "Page &p", footer: "&p");

        var result = fixture.Paginator.GetPage(0);
        var texts = GetDrawTexts(Assert.IsType<DrawingVisual>(
            Assert.IsType<ContainerVisual>(result.Visual).Children[1]));

        Assert.Contains("Page 1", texts);
        Assert.Contains("1", texts);
    }

    [StaFact(DisplayName = "【正常系】ヘッダー・フッター:マクロを含まない文字列はそのまま描画されること")]
    public void GetPage_WithoutMacros_ShouldDrawTextAsIs()
    {
        var fixture = new PaginatorFixture(header: "ヘッダー", footer: "フッター");

        var result = fixture.Paginator.GetPage(0);
        var texts = GetDrawTexts(Assert.IsType<DrawingVisual>(
            Assert.IsType<ContainerVisual>(result.Visual).Children[1]));

        Assert.Contains("ヘッダー", texts);
        Assert.Contains("フッター", texts);
    }
    #endregion

    #region プロパティ委譲
    [StaFact(DisplayName = "【正常系】IsPageCountValid:ベースのページャーの値が返ること")]
    public void IsPageCountValid_ShouldDelegateToBasePaginator()
    {
        var fixture = new PaginatorFixture();
        fixture.BasePaginator.IsPageCountValidValue = false;

        Assert.False(fixture.Paginator.IsPageCountValid);
    }

    [StaFact(DisplayName = "【正常系】PageCount:ベースのページャーの値が返ること")]
    public void PageCount_ShouldDelegateToBasePaginator()
    {
        var fixture = new PaginatorFixture();
        fixture.BasePaginator.PageCountValue = 12;

        Assert.Equal(12, fixture.Paginator.PageCount);
    }

    [StaFact(DisplayName = "【正常系】PageSize:取得・設定がベースのページャーに委譲されること")]
    public void PageSize_ShouldDelegateToBasePaginator()
    {
        var fixture = new PaginatorFixture();
        var newSize = new Size(1024, 768);

        fixture.Paginator.PageSize = newSize;

        Assert.Equal(newSize, fixture.BasePaginator.PageSize);
        Assert.Equal(newSize, fixture.Paginator.PageSize);
    }

    [StaFact(DisplayName = "【正常系】Source:ベースのページャーの値が返ること")]
    public void Source_ShouldDelegateToBasePaginator()
    {
        var fixture = new PaginatorFixture();

        Assert.Same(fixture.BasePaginator.Source, fixture.Paginator.Source);
    }
    #endregion
}
