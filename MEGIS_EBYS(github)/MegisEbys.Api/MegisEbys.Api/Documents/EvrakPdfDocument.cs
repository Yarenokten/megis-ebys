using MegisEbys.Api.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MegisEbys.Api.Documents;

public class EvrakPdfDocument : IDocument
{
    private readonly Evrak _evrak;

    public EvrakPdfDocument(Evrak evrak)
    {
        _evrak = evrak;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(50);
                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
    }

    void ComposeHeader(IContainer container)
    {
        var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text($"Evrak No: {_evrak.EvrakNo}").SemiBold();
                column.Item().Text($"Tarih: {_evrak.Tarih:dd.MM.yyyy HH:mm}");
            });

            row.ConstantItem(100).Height(50).Placeholder(); // Logo için boşluk
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(40).Column(column =>
        {
            column.Spacing(10);

            var gonderen = _evrak.DahiliMi ? _evrak.GonderenKullanici?.AdSoyad : _evrak.IlgiliKurum;
            var alici = _evrak.DahiliMi ? (_evrak.AliciKullanici?.AdSoyad ?? _evrak.AliciBirim?.Ad) : "Kurumumuz";

            column.Item().Text($"Konu: {_evrak.Konu}").FontSize(14).Bold();
            column.Item().LineHorizontal(1);

            column.Item().Text($"Gönderen: {gonderen}");
            column.Item().Text($"Alıcı: {alici}");

            column.Item().PaddingTop(20).Text(_evrak.Konu); // Gerçek bir sistemde burası evrakın metni olur.
        });
    }
}