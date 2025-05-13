using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Comunicazioni.Models.Entities
{
    public class Docente
    {
        [Key]
        public Guid K_Docente { get; set; }
        public string? Email { get; set; }
        public string? PWD { get; set; }
        public string? Cognome { get; set; }
        public string? Nome { get; set; }
        public DateTime? DataNascita { get; set; }
        public string? Indirizzo { get; set; }
        public string? CAP { get; set; }
        public string? Citta { get; set; }
        public string? Provincia { get; set; }
        public byte[]? ImmagineProfilo { get; set; }
        public string? Tipo { get; set; }
        public DateTime? DataRegistrazione { get; set; }
        public string? Abilitato { get; set; }
        //per accedere agli esami dei docenti
        public virtual ICollection<Esame>? Esami { get; set; }
    }
}
