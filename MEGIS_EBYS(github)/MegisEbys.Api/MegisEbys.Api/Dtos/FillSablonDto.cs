using System.Collections.Generic;

namespace MegisEbys.Api.Dtos;

public class FillSablonDto
{
    // Örn: Key = "[Konu]", Value = "Kar Tatili Hakkında"
    public Dictionary<string, string> Placeholders { get; set; }
}
