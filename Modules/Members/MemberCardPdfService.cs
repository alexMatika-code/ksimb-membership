using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ksimb_membership.Modules.Members;

internal sealed class MemberCardPdfService
{
    private const float CardWidth = 85.60f;
    private const float CardHeight = 53.98f;

    public byte[] Generate(IEnumerable<Member> members)
    {
        var membersWithCards = members
            .Where(x => x.MemberCardNumber.HasValue)
            .OrderBy(x => x.MemberCardNumber)
            .ToList();

        return Document.Create(document =>
        {
            foreach (var batch in membersWithCards.Chunk(8))
            {
                AddFrontPage(document, batch);
                AddBackPage(document, batch);
            }
        }).GeneratePdf();
    }

    private static void AddFrontPage(
        IDocumentContainer document,
        IReadOnlyCollection<Member> members)
    {
        document.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(10, Unit.Millimetre);

            page.Content()
                .AlignCenter()
                .AlignMiddle()
                .Grid(grid =>
                {
                    grid.Columns(2);

                    foreach (var member in members)
                    {
                        grid.Item()
                            .Width(CardWidth, Unit.Millimetre)
                            .Height(CardHeight, Unit.Millimetre)
                            .Element(container => DrawFront(container, member));
                    }
                });
        });
    }

    private static void AddBackPage(
        IDocumentContainer document,
        IReadOnlyCollection<Member> members)
    {
        /*
         * Kod duplex printanja raspored poleđine mora odgovarati
         * rasporedu prednje strane.
         *
         * Za 2 stupca zamjenjujemo lijevu/desnu karticu:
         *
         * FRONT:       BACK:
         *
         * 1 | 2        2 | 1
         * 3 | 4        4 | 3
         * 5 | 6        6 | 5
         * 7 | 8        8 | 7
         */

        var mirrored = members
            .Chunk(2)
            .SelectMany(row => row.Reverse())
            .ToList();

        document.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(10, Unit.Millimetre);

            page.Content()
                .AlignCenter()
                .AlignMiddle()
                .Grid(grid =>
                {
                    grid.Columns(2);

                    foreach (var member in mirrored)
                    {
                        grid.Item()
                            .Width(CardWidth, Unit.Millimetre)
                            .Height(CardHeight, Unit.Millimetre)
                            .Element(container => DrawBack(container, member));
                    }
                });
        });
    }

    private static void DrawFront(IContainer container, Member member)
    {
        container
            .Border(0.5f)
            .Padding(5, Unit.Millimetre)
            .Column(column =>
            {
                column.Spacing(2, Unit.Millimetre);

                column.Item()
                    .Text("KSIMB")
                    .Bold()
                    .FontSize(16);

                column.Item()
                    .Text(member.FullName)
                    .Bold()
                    .FontSize(12);

                column.Item()
                    .Text($"Članski broj: {member.MemberCardNumber:D4}")
                    .FontSize(10);
            });
    }

    private static void DrawBack(IContainer container, Member member)
    {
        container
            .Border(0.5f)
            .Padding(5, Unit.Millimetre)
            .Column(column =>
            {
                column.Spacing(2, Unit.Millimetre);

                column.Item()
                    .Text("KSIMB")
                    .Bold()
                    .FontSize(14);

                column.Item()
                    .Text($"Član: {member.FullName}")
                    .FontSize(9);

                column.Item()
                    .Text($"Broj: {member.MemberCardNumber:D4}")
                    .FontSize(9);

                column.Item()
                    .PaddingTop(5)
                    .Text("Poleđina članske iskaznice")
                    .FontSize(8);
            });
    }
}