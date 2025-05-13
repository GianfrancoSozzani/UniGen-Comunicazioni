using System.ComponentModel.DataAnnotations;
using   Comunicazioni.Data;
using Comunicazioni.Models.Entities;
namespace Comunicazioni.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La password è obbligatoria")]
        [MinLength(5, ErrorMessage = "La password deve contenere almeno 5 caratteri")]
        public string? Password { get; set; }

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
    }

}

