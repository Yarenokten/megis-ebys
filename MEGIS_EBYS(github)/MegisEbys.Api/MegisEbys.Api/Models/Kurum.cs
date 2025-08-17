namespace MegisEbys.Api.Models;

public class Kurum
{
    public int Id { get; set; }
    public string Ad { get; set; }

    // Artık DisKullanici yerine DisDepartman koleksiyonu tutuyor
    public ICollection<DisDepartman> Departmanlar { get; set; } = new List<DisDepartman>();
}
