using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MegisEbys.Api.Dtos;

public class AiPromptDto
{
    [Required]
    public string Prompt { get; set; }
    public List<string> Placeholders { get; set; }
}
