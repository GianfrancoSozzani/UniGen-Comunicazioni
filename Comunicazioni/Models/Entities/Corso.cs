using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Comunicazioni.Models.Entities
{
    public class Corso
    {
        [Key]
        public Guid K_Corso { get; set; }
        public string? TitoloCorso { get; set; }
        public Guid K_Facolta { get; set; }
        [ForeignKey("K_Facolta")]
        [ValidateNever]
        public Facolta? TitoloFacolta { get; set; }
        public Guid K_TipoCorso { get; set; }
        [ForeignKey("K_TipoCorso")]
        [ValidateNever]
        public TipiCorsi? Tipo { get; set; }
        public string? MinimoCFU { get; set; }
    }
}
