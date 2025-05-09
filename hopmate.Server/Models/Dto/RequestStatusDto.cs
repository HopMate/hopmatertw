using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Dto
{
    public class RequestStatusDto
    {
        [Required]
        public string Status { get; set; } = null!;
    }
}
