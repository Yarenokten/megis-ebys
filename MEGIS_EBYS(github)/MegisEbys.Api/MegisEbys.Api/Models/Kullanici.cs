using System.ComponentModel.DataAnnotations;

namespace MegisEbys.Api.Models;

public class Kullanici
{
    public int Id { get; set; }
    [Required]
    public string AdSoyad { get; set; }
    [Required]
    [EmailAddress]
    public string Eposta { get; set; }

    // [Required] attribute'u kaldırıldı ve string nullable yapıldı (?)
    public string? Sifre { get; set; }

    public KullaniciYetki Yetki { get; set; }
    public bool AktifMi { get; set; } = true;

    public int? BirimId { get; set; } // Kullanıcının bağlı olduğu birim
    public Birim? Birim { get; set; }
}