using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace Comunicazioni.Models.Entities
{
    public class TipiCorsi
    {
        [Key]
        public Guid K_Tipo_Corso { get; set; }
        public string? Tipo { get; set; }
        public string? Durata { get; set; }
    }
}
