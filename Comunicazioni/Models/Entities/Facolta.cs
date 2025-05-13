using System.ComponentModel.DataAnnotations;

namespace Comunicazioni.Models.Entities
{
    public class Facolta
    {
        [Key]
        public Guid K_Facolta { get; set; }
        public string? TitoloFacolta { get; set; }
    }
}
