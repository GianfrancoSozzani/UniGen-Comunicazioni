using Comunicazioni.Models.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comunicazioni.Models
{
    public class AddComunicazioneViewModel
    {
        public DateTime DataOraComunicazione { get; set; }
        public string? Soggetto { get; set; }
        public string? Testo { get; set; }
        public Guid? K_Studente { get; set; }
        public Guid? K_Docente { get; set; }
        public Guid? K_Esame { get; set; }
    }
}
