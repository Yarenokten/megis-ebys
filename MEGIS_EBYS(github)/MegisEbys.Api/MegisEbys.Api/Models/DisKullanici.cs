using System.ComponentModel.DataAnnotations;

namespace MegisEbys.Api.Models;

public class DisKullanici
{
    public int Id { get; set; }
    [Required]
    public string AdSoyad { get; set; }
    [EmailAddress]
    public string? Eposta { get; set; }
    public string? Unvan { get; set; }

    // KurumId yerine DisDepartmanId geldi
    public int DisDepartmanId { get; set; }
    public DisDepartman? DisDepartman { get; set; }
}
