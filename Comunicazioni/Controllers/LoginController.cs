using Comunicazioni.Data;
using Comunicazioni.Models;
using Comunicazioni.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Importante per la sessione

namespace Comunicazioni.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public LoginController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Controllo login Studente
            var studente = await dbContext.Studenti
                .Where(s => s.Email == viewModel.username && s.PWD == viewModel.PWD)
                .Select(s => new { s.K_Studente, s.Email, s.Matricola, s.Abilitato }) // Carico solo K_Studente ed Email e Matricola
                .FirstOrDefaultAsync();


            if (studente != null && studente.Matricola != null && studente.Abilitato == "S")
            {
                return RedirectToAction("List", "Comunicazioni", new { cod = studente.K_Studente, usr = studente.Email, r = "s", mat = studente.Matricola, a = studente.Abilitato });
            }


            // Controllo login Docente
            var docente = await dbContext.Docenti
                .Where(s => s.Email == viewModel.username && s.PWD == viewModel.PWD)
                .Select(s => new { s.K_Docente, s.Email, s.Abilitato }) // Carico solo K_Docente ed Email e Abilitato
                .FirstOrDefaultAsync();

            if (docente != null && docente.Abilitato != "N")
            {
                return RedirectToAction("List", "Comunicazioni", new { cod = docente.K_Docente, usr = docente.Email, r = "d" });
            }
            //Controllo login Operatore dell' Amministrazione
            var operatore = await dbContext.Operatori
                .Where(o => o.USR == viewModel.username && o.PWD == viewModel.PWD)
                .Select(o => new { o.K_Operatore, o.USR }) // Carico solo K_Studente ed Email e Matricola
                .FirstOrDefaultAsync();

            if (operatore != null)
            {
                return RedirectToAction("List", "Comunicazioni", new { cod = operatore.K_Operatore, usr = operatore.USR, r = "a" });
            }

            // Nessun utente trovato
            ModelState.AddModelError("", "Credenziali non valide.");
            return View(viewModel);
        }
    }
}