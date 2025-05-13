using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Comunicazioni.Models.Entities
{
    public class PianoStudioPersonale
    {
        [Key]
        public Guid K_PianoStudioPersonale { get; set; }
        public Guid? K_Esame { get; set; }
        [ForeignKey("K_Esame")]
        [ValidateNever]
        public Esame? Esame { get; set; }
        public string? AnnoAccademico { get; set; }
        public string? Obbligatorio { get; set; }
        public Guid K_Studente { get; set; }
        [ForeignKey("K_Studente")]
        [ValidateNever]
        public Studente? Studente { get; set; }
    }
}
