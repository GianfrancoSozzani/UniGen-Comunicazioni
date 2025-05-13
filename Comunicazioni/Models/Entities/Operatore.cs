using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Comunicazioni.Models.Entities
{
    public class Operatore
    {
        [Key]
        public Guid K_Operatore { get; set; }
        public string? USR { get; set; }
        public string? PWD { get; set; }
        public string? Nome { get; set; }
        public string? Cognome { get; set; }
    }
}
