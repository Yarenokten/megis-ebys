namespace MegisEbys.Api.Models;

public class Evrak
{
    public int Id { get; set; }
    public EvrakYonu Yonu { get; set; }

    // DÜZELTME: [Required] attribute'u kaldırıldı ve string nullable yapıldı (?)
    public string? EvrakNo { get; set; }

    public string Konu { get; set; }

    public string? IlgiliKurum { get; set; }

    public DateTime Tarih { get; set; }
    public EvrakDurum Durum { get; set; }

    public int EvrakTuruId { get; set; }
    public EvrakTuru? EvrakTuru { get; set; }

    public int? SorumluBirimId { get; set; }
    public Birim? SorumluBirim { get; set; }

    public bool DahiliMi { get; set; } = false;

    public int? GonderenKullaniciId { get; set; }
    public Kullanici? GonderenKullanici { get; set; }

    public int? AliciKullaniciId { get; set; }
    public Kullanici? AliciKullanici { get; set; }

    public int? AliciBirimId { get; set; }
    public Birim? AliciBirim { get; set; }

    public string? DosyaYolu { get; set; }
}
