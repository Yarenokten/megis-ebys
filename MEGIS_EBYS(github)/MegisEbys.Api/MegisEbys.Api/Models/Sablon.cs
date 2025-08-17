using System.ComponentModel.DataAnnotations;

namespace MegisEbys.Api.Models;

public class Sablon
{
    public int Id { get; set; }

    [Required]
    public string Ad { get; set; } // Örn: "Talep Yazısı Şablonu"

    [Required]
    public string DosyaYolu { get; set; } // Sunucudaki .docx dosyasının yolu
}