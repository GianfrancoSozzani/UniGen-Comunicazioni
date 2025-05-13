using Microsoft.AspNetCore.Mvc;

namespace Comunicazioni.Models
{
    public class ListAndAddViewModel
    {
        public List<IGrouping<Guid, Comunicazioni.Models.Entities.Comunicazione>>? Comunicazioni { get; set; }
        [BindProperty]
        public AddComunicazioneViewModel? AddComunicazione { get; set; }
    }
}
