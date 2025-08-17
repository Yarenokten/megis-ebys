using System.ComponentModel.DataAnnotations;

namespace MegisEbys.Api.Models;

public class IzinTalebi
{
    public int Id { get; set; }

    [Required]
    public DateTime BaslangicTarihi { get; set; }
    [Required]
    public DateTime BitisTarihi { get; set; }
    [Required]
    public string Aciklama { get; set; }
    public IzinDurum Durum { get; set; } = IzinDurum.OnayBekliyor;
    public DateTime TalepTarihi { get; set; } = DateTime.Now;

    public int TalepEdenKullaniciId { get; set; }
    public Kullanici? TalepEdenKullanici { get; set; }
}