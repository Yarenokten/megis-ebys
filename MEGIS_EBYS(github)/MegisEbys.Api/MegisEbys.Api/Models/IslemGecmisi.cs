using System.ComponentModel.DataAnnotations;

namespace MegisEbys.Api.Models;

public class IslemGecmisi
{
    public int Id { get; set; }

    [Required]
    public string Aciklama { get; set; }
    public DateTime IslemTarihi { get; set; }

    public int EvrakId { get; set; }
    public Evrak? Evrak { get; set; }

    public int? KullaniciId { get; set; }
    public Kullanici? Kullanici { get; set; }
}