using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ksimb_membership.Modules.Members;

internal sealed class MemberCardPdfService
{
    private const int CardsPerPage = 10;

    private const float CardWidth = 85.6f;
    private const float CardHeight = 53.98f;

    private const float HorizontalGap = 1f;
    private const float VerticalGap = 0.5f;

    private const float PageLeft = 14f;
    private const float PageTop = 8f;

    private const float FirstNameX = 13f;
    private const float FirstNameY = 9.5f;

    private const float LastNameX = 20f;
    private const float LastNameY = 15.3f;

    private const float CardNumberX = 28f;
    private const float CardNumberY = 20.8f;

    private readonly IWebHostEnvironment _environment;

    public MemberCardPdfService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public byte[] Generate(IEnumerable<Member> members)
    {
        var membersWithCards = members
            .Where(x => x.MemberCardNumber.HasValue)
            .OrderBy(x => x.MemberCardNumber)
            .ToList();

        var frontPath = Path.Combine(
            _environment.WebRootPath,
            "assets",
            "front.png");

        var backPath = Path.Combine(
            _environment.WebRootPath,
            "assets",
            "back.png");
        return Document.Create(document =>
        {
            foreach (var batch in membersWithCards.Chunk(CardsPerPage))
            {
                var cards = batch.ToList();
                AddFrontPage(document, cards, frontPath);
                AddBackPage(document, cards, backPath);
            }
        }).GeneratePdf();
    }

    private static void AddFrontPage(
        IDocumentContainer document,
        IReadOnlyList<Member> members,
        string frontPath)
    {
        document.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(0);

            page.Content()
                .Element(container =>
                    DrawPage(container, members, frontPath, true));
        });
    }

    private static void AddBackPage(
        IDocumentContainer document,
        IReadOnlyList<Member> members,
        string backPath)
    {
        document.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(0);

            page.Content()
                .Element(container =>
                    DrawPage(container, members, backPath, false));
        });
    }

    private static void DrawPage(
        IContainer container,
        IReadOnlyList<Member> members,
        string imagePath,
        bool isFront)
    {
        container
            .PaddingTop(PageTop, Unit.Millimetre)
            .PaddingLeft(PageLeft, Unit.Millimetre)
            .Column(column =>
            {
                column.Spacing(VerticalGap, Unit.Millimetre);

                for (var row = 0; row < 5; row++)
                {
                    column.Item().Row(rowContainer =>
                    {
                        rowContainer.Spacing(HorizontalGap, Unit.Millimetre);

                        for (var col = 0; col < 2; col++)
                        {
                            var index = row * 2 + col;

                            rowContainer
                                .ConstantItem(CardWidth, Unit.Millimetre)
                                .Height(CardHeight, Unit.Millimetre)
                                .Element(card =>
                                {
                                    if (index >= members.Count)
                                        return;

                                    if (isFront)
                                        DrawFront(card, members[index], imagePath);
                                    else
                                        DrawBack(card, imagePath);
                                });
                        }
                    });
                }
            });
    }

    private static void DrawFront(
        IContainer container,
        Member member,
        string frontPath)
    {
        container.Layers(layers =>
        {
            layers.PrimaryLayer()
                .Image(frontPath)
                .FitArea();

            AddCardText(
                layers,
                member.FirstName,
                FirstNameX,
                FirstNameY);

            AddCardText(
                layers,
                member.LastName,
                LastNameX,
                LastNameY);

            AddCardText(
                layers,
                $"{member.MemberCardNumber:000}",
                CardNumberX,
                CardNumberY);
        });
    }

    private static void AddCardText(
        LayersDescriptor layers,
        string text,
        float x,
        float y)
    {
        layers.Layer()
            .PaddingLeft(x, Unit.Millimetre)
            .PaddingTop(y, Unit.Millimetre)
            .Text(text)
            .FontFamily("Georgia")
            .FontSize(7)
            .SemiBold();
    }

    private static void DrawBack(
        IContainer container,
        string backPath)
    {
        container
            .Image(backPath)
            .FitArea();
    }
}