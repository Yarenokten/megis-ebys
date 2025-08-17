namespace MegisEbys.Api.Models;

public class DisDepartman
{
    public int Id { get; set; }
    public string Ad { get; set; }

    public int KurumId { get; set; }
    // DÜZELTME: Bu satıra '?' eklenerek derleme hatası giderildi.
    public Kurum? Kurum { get; set; }

    public ICollection<DisKullanici> Kullanicilar { get; set; } = new List<DisKullanici>();
}