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
                .Select(s => new { s.K_Studente, s.Email, s.Matricola }) // Carico solo K_Studente ed Email e Matricola
                .FirstOrDefaultAsync();


            if (studente != null)
            {
                // Salvo solo ciò che serve
                //HttpContext.Session.SetString("K_Studente", studente.K_Studente.ToString());
                //HttpContext.Session.SetString("Email", studente.Email);
                //HttpContext.Session.SetString("Ruolo", "S");

                if (studente.Matricola == null)
                {
                    //return RedirectToAction("AREA STUDENTE (NON IMMATRICOLATO)", "Home");
                    return Redirect("https://localhost:7050/Studenti/ModificaProfilo?cod=" + studente.K_Studente.ToString() + "&&usr=" + studente.Email + "&&r=s");

                }

                //return RedirectToAction("AREA LAVORO STUDENTE (IMMATRICOLATO)", "Home");
                return Redirect("https://localhost:7098/Home/Index?cod=" + studente.K_Studente.ToString() + "&&usr=" + studente.Email + "&&r=s");

            }
            //if (studente != null)
            //{
            //    // Salvo solo ciò che serve
            //    //HttpContext.Session.SetString("K_Studente", studente.K_Studente.ToString());
            //    //HttpContext.Session.SetString("Email", studente.Email);
            //    //HttpContext.Session.SetString("Ruolo", "S");


            //    //return RedirectToAction("AREA LAVORO STUDENTE (IMMATRICOLATO)", "Home");
            //    HttpContext.Session.SetString("studente_id", studente.K_Studente.ToString().ToUpper());
            //    return Redirect("https://localhost:7050/Studenti/Show/?cod="+studente.K_Studente.ToString().ToUpper());


            //}

            // Controllo login Docente
            var docente = await dbContext.Docenti
                .Where(s => s.Email == viewModel.username && s.PWD == viewModel.PWD)
                .Select(s => new { s.K_Docente, s.Email, s.Abilitato }) // Carico solo K_Docente ed Email e Abilitato
                .FirstOrDefaultAsync();

            if (docente != null)
            {
                //HttpContext.Session.SetString("K_Docente", docente.K_Docente.ToString());
                //HttpContext.Session.SetString("Email", docente.Email);
                //HttpContext.Session.SetString("Ruolo", "D");
                if (docente.Abilitato == "N")
                {
                    return Redirect("http://localhost:5201/Studenti/ModificaProfilo?cod=" + docente.K_Docente.ToString() + "&&usr=" + docente.Email + "&&r=d");
                    //return RedirectToAction("AREA DOCENTE (NON ABILITATO)", "Home");
                }
                return Redirect("https://localhost:7098/Home/Index?cod=" + docente.K_Docente.ToString() + "&&usr=" + docente.Email + "&&r=d");
                //return RedirectToAction("AREA DOCENTE (ABILITATO)", "Home");
            }

            var operatore = await dbContext.Operatori
            .Where(o => o.USR == viewModel.username && o.PWD == viewModel.PWD)
            .Select(o => new { o.K_Operatore, o.USR }) // Carico solo K_Studente ed Email e Matricola
            .FirstOrDefaultAsync();

            if (operatore != null)
            {
                // Salvo solo ciò che serve
                //HttpContext.Session.SetString("K_Operatore", operatore.K_Operatore.ToString());
                //HttpContext.Session.SetString("USR", operatore.USR);
                //HttpContext.Session.SetString("Ruolo", "O");


                return Redirect("https://localhost:7098/Home/Index?cod=" + operatore.K_Operatore.ToString() + "&&usr=" + operatore.USR + "&&r=a");

                //return RedirectToAction("AREA AMMINISTRAZIONE", "Home");
            }

            // Nessun utente trovato
            ModelState.AddModelError("", "Credenziali non valide.");
            return RedirectToAction("Privacy", "Home");
        }



        //public IActionResult Register()
        //{
        //    return View();
        //}


        //[HttpPost]
        //public async Task<IActionResult> Register(RegisterViewModel viewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(viewModel);
        //    }

        //    // Elaborazione immagine profilo
        //    if (viewModel.ImmagineFile != null)
        //    {
        //        using var ms = new MemoryStream();
        //        await viewModel.ImmagineFile.CopyToAsync(ms);
        //        viewModel.ImmagineProfilo = ms.ToArray();
        //        viewModel.Tipo = Path.GetExtension(viewModel.ImmagineFile.FileName).ToLowerInvariant();
        //    }

        //    var corso = await dbContext.Corsi.FirstOrDefaultAsync(); // solo per k_corso
        //    if (corso == null)
        //    {
        //        ModelState.AddModelError("", "Nessun corso disponibile.");
        //        return View(viewModel);
        //    }

        //    var studente = new Studente
        //    {
        //        Nome = viewModel.Nome?.Trim(),
        //        Cognome = viewModel.Cognome?.Trim(),
        //        Email = viewModel.Email?.Trim(),
        //        PWD = viewModel.Password,
        //        DataNascita = viewModel.DataNascita ?? DateTime.Now,
        //        Indirizzo = viewModel.Indirizzo,
        //        CAP = viewModel.CAP,
        //        Citta = viewModel.Citta,
        //        Provincia = viewModel.Provincia,
        //        ImmagineProfilo = viewModel.ImmagineProfilo,
        //        Tipo = viewModel.Tipo,
        //        Abilitato = "No",
        //        DataImmatricolazione = null,
        //        K_Corso = corso.K_Corso
        //    };

        //    await dbContext.Studenti.AddAsync(studente);
        //    await dbContext.SaveChangesAsync();

        //    return RedirectToAction("Index", "FAQ");
        //}
    }
}