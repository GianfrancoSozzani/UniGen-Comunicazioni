using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Comunicazioni.Models.Entities
{
    public class Studente
    {
        [Key]
        public Guid K_Studente { get; set; }
        public string? Email { get; set; }
        public string? PWD { get; set; }
        public string? Nome { get; set; }
        public string? Cognome { get; set; }
        public DateTime? DataNascita { get; set; }
        public string? Indirizzo { get; set; }
        public string? CAP { get; set; }
        public string? Citta { get; set; }
        public string? Provincia { get; set; }
        public byte[]? ImmagineProfilo { get; set; }
        public string? Tipo { get; set; }
        public int? Matricola { get; set; }
        public DateTime? DataImmatricolazione { get; set; }
        public Guid? K_Corso { get; set; }
        [ForeignKey("K_Corso")]
        [ValidateNever]
        public Corso? Corso { get; set; }
        public string? Abilitato { get; set; }
    }
}
