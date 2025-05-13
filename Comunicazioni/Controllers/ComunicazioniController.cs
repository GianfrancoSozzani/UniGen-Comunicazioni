using System;
using System.Linq;
using System.Net.Mail;
using Comunicazioni.Data;
using Comunicazioni.Models;
using Comunicazioni.Models.Entities;
using LibreriaClassi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Comunicazioni.Controllers
{
    public class ComunicazioniController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public ComunicazioniController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //---- View Bags per i select list ----//
        public void PopolaStudenti()
        {
            string ruolo = HttpContext.Session.GetString("r");
            if (ruolo == "a")
            {
                IEnumerable<SelectListItem> listaStudenti = dbContext.Studenti
                    .Select(i => new SelectListItem
                    {
                        Text = i.Nome + " " + i.Cognome,
                        Value = i.K_Studente.ToString()
                    });
                ViewBag.StudentiList = listaStudenti;
            }
            else if (ruolo == "d")
            {
                Guid docenteId = Guid.Parse(HttpContext.Session.GetString("cod"));

                // Ottieni gli ID degli esami del docente
                var esamiDelDocenteList = dbContext.Esami
                    .Where(e => e.K_Docente == docenteId)
                    .Select(e => e.K_Esame)
                    .ToList();

                // Ottieni solo gli studenti che hanno almeno un esame del docente in PianiStudioPersonali
                var studentiFiltrati = dbContext.PianiStudioPersonali
                    .Where(ps => ps.K_Esame.HasValue && esamiDelDocenteList.Contains(ps.K_Esame.Value))
                    .Select(ps => ps.K_Studente)
                    .Distinct()
                    .ToList();

                // Carica solo questi studenti dalla tabella Studenti
                var listaStudenti = dbContext.Studenti
                    .Where(s => studentiFiltrati.Contains(s.K_Studente) && s.Matricola != null)
                    .OrderBy(s => s.Cognome)
                    .Select(s => new SelectListItem
                    {
                        Text = s.Cognome + " " + s.Nome,
                        Value = s.K_Studente.ToString()
                    })
                    .ToList();

                ViewBag.StudentiList = listaStudenti;
            }
        }

        public void PopolaDocenti()
        {
            string ruolo = HttpContext.Session.GetString("r");
            if (ruolo == "a")
            {
                IEnumerable<SelectListItem> listaDocenti = dbContext.Docenti
                    .OrderBy(docente => docente.Cognome)
                    .Select(i => new SelectListItem
                    {
                        Text = i.Nome + " " + i.Cognome,
                        Value = i.K_Docente.ToString()
                    });
                ViewBag.DocentiList = listaDocenti;
            }
            else if (ruolo == "s")
            {
                var Idstudente = Guid.Parse(HttpContext.Session.GetString("cod"));
                var pianiDiStudio = dbContext.PianiStudioPersonali
                .Where(ps => ps.K_Studente == Idstudente)
                .Select(ps => ps.K_Esame);

                var listaDocenti = dbContext.Esami
                       .Where(e => pianiDiStudio.Contains(e.K_Esame))
                       .Select(e => e.Docente)
                       .Distinct()
                       .OrderBy(docente => docente.Cognome)
                       .Select(docente => new SelectListItem
                       {
                           Text = docente.Cognome + " " + docente.Nome,
                           Value = docente.K_Docente.ToString()
                       }).ToList();  // Converti in lista

                ViewBag.DocentiList = listaDocenti;
            }
        }

        //----------------------------------------------//
        //LIST------------------------------------------//
        //----------------------------------------------//

        //metodo per recuperare i dati del mittente e del destinatario
        private async Task DatiMittenteDestinatario(List<IGrouping<Guid, Comunicazioni.Models.Entities.Comunicazione>> comunicazioni)
        {
            foreach (var gruppo in comunicazioni)
            {
                foreach (var comunicazione in gruppo)
                {
                    // Carica il mittente
                    comunicazione.MittenteStudente = null;
                    comunicazione.MittenteDocente = null;
                    if (comunicazione.K_Soggetto.HasValue)
                    {
                        comunicazione.MittenteStudente = await dbContext.Studenti
                            .FirstOrDefaultAsync(s => s.K_Studente == comunicazione.K_Soggetto);
                        if (comunicazione.MittenteStudente == null)
                        {
                            comunicazione.MittenteDocente = await dbContext.Docenti
                                .FirstOrDefaultAsync(d => d.K_Docente == comunicazione.K_Soggetto);
                        }
                    }

                    // Carica il destinatario
                    comunicazione.DestinatarioStudente = null;
                    if (comunicazione.K_Studente.HasValue)
                    {
                        comunicazione.DestinatarioStudente = await dbContext.Studenti
                            .FirstOrDefaultAsync(s => s.K_Studente == comunicazione.K_Studente);
                    }
                    else if (comunicazione.K_Docente.HasValue)
                    {
                        comunicazione.DestinatarioDocente = await dbContext.Docenti
                            .FirstOrDefaultAsync(d => d.K_Docente == comunicazione.K_Docente);
                    }
                }
            }
        }
        //----------------------------------------------//
        [HttpGet]
        public async Task<IActionResult> List(string r, string cod, string? mat, string? a, string? usr)
        {
            HttpContext.Session.SetString("r", r);
            HttpContext.Session.SetString("cod", cod);
            if (r == "s")
            {
                HttpContext.Session.SetString("mat", mat);
                HttpContext.Session.SetString("a", a);
            }
            if (r == "a")
            {
                HttpContext.Session.SetString("usr", usr);
            }

            string ruolo = HttpContext.Session.GetString("r");
            List<IGrouping<Guid, Comunicazione>> gruppo_comunicazioni;

            PopolaStudenti();
            PopolaDocenti();

            var viewModel = new Comunicazioni.Models.ListAndAddViewModel();

            if (ruolo == "s")
            {
                var studente_chiave = Guid.Parse(HttpContext.Session.GetString("cod"));

                gruppo_comunicazioni = await dbContext.Comunicazioni
                    .Where(c => c.K_Studente == studente_chiave || c.K_Soggetto == studente_chiave)
                    .OrderBy(c => c.DataOraComunicazione)
                    .GroupBy(c => c.Codice_Comunicazione)
                    .ToListAsync();

                await DatiMittenteDestinatario(gruppo_comunicazioni);

                viewModel.Comunicazioni = gruppo_comunicazioni;

                return View(viewModel);
            }
            //Logica simile per il ruolo "Docente" e "Amministrazione"
            else if (ruolo == "d")
            {
                var docente_chiave = Guid.Parse(HttpContext.Session.GetString("cod"));

                gruppo_comunicazioni = await dbContext.Comunicazioni
                    .Where(c => c.K_Docente == docente_chiave || c.K_Soggetto == docente_chiave)
                    .OrderBy(c => c.DataOraComunicazione)
                    .GroupBy(c => c.Codice_Comunicazione)
                    .ToListAsync();

                await DatiMittenteDestinatario(gruppo_comunicazioni);

                viewModel.Comunicazioni = gruppo_comunicazioni;

                return View(viewModel);
            }
            else
            {

                // Recupera i Codice_Comunicazione dei messaggi inviati e ricevuti dall'amministrazione
                var codiciComunicazioneAmministrazione = await dbContext.Comunicazioni
                    .Where(c => (c.K_Studente == null && c.K_Docente == null) || dbContext.Operatori.Any(o => o.K_Operatore == c.K_Soggetto))
                    .Select(c => c.Codice_Comunicazione)
                    .Distinct()
                    .ToListAsync();

                // Recupera tutte le comunicazioni che hanno un Codice_Comunicazione appartenente alle comunicazioni amministrative
                gruppo_comunicazioni = await dbContext.Comunicazioni
                    .Include(c => c.Studente)
                    .Include(c => c.Docente)
                    .Where(c => codiciComunicazioneAmministrazione.Contains(c.Codice_Comunicazione)) // Qui prendiamo sia le comunicazioni amministrative che le risposte
                    .OrderBy(c => c.DataOraComunicazione)
                    .GroupBy(c => c.Codice_Comunicazione)
                    .ToListAsync();

                await DatiMittenteDestinatario(gruppo_comunicazioni);

                viewModel.Comunicazioni = gruppo_comunicazioni;

                return View(viewModel);

            }

        }

        //----------------------------------------------//
        //ADD-------------------------------------------//
        //----------------------------------------------//


        //metodo per inviare email alla creazione di una comunicazione

        private async Task InvioEmail(Comunicazione comunicazione, string ruolo, string subject)
        {
            List<string> destinatariEmail = new List<string>();

            // Determina i destinatari in base al ruolo e ai campi K_Studente/K_Docente
            if (ruolo == "a")
            {
                // L'operatore potrebbe inviare a uno studente o a un docente specifico
                if (comunicazione.K_Studente.HasValue && comunicazione.Studente?.Email != null)
                {
                    destinatariEmail.Add(comunicazione.Studente.Email);
                }
                else if (comunicazione.K_Docente.HasValue && comunicazione.Docente?.Email != null)
                {
                    destinatariEmail.Add(comunicazione.Docente.Email);
                }
            }
            else if (ruolo == "d")
            {
                // Il docente potrebbe inviare a uno studente specifico
                if (comunicazione.K_Studente.HasValue && comunicazione.Studente?.Email != null)
                {
                    destinatariEmail.Add(comunicazione.Studente.Email);
                }
                else
                {
                    destinatariEmail.Add("generation@brovia.it");
                }
            }
            else if (ruolo == "s")
            {
                // Lo studente potrebbe inviare a un docente specifico
                if (comunicazione.K_Docente.HasValue && comunicazione.Docente?.Email != null)
                {
                    destinatariEmail.Add(comunicazione.Docente.Email);
                }
                else
                {
                    destinatariEmail.Add("generation@brovia.it");
                }
            }

            // Invia email se ci sono destinatari
            if (destinatariEmail.Any())
            {
                using (SmtpClient smtpClient = new SmtpClient("mail.brovia.it", 587))
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential("generation@brovia.it", "G3n3rat!on");
                    smtpClient.EnableSsl = true; // Generalmente è meglio usare sempre SSL

                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress("generation@brovia.it", "Comunicazione UniGen");

                        foreach (var email in destinatariEmail.Distinct())
                        {
                            mail.To.Add(new MailAddress(email));
                        }

                        mail.Subject = subject;
                        string body = "";

                        if (ruolo == "d")
                        {
                            var docente = await dbContext.Docenti.FirstOrDefaultAsync(d => d.K_Docente == comunicazione.K_Soggetto);
                            body = @$"In data {comunicazione.DataOraComunicazione}
hai ricevuto una comunicazione da {docente?.Nome} {docente?.Cognome}.

{comunicazione.Testo}";
                        }
                        else if (ruolo == "s")
                        {
                            var studente = await dbContext.Studenti.FirstOrDefaultAsync(s => s.K_Studente == comunicazione.K_Soggetto);
                            body = @$"In data {comunicazione.DataOraComunicazione}
hai ricevuto una comunicazione da {studente?.Nome} {studente?.Cognome}.

{comunicazione.Testo}";
                        }
                        else
                        {
                            body = @$"In data {comunicazione.DataOraComunicazione}
hai ricevuto una comunicazione dall'Amministrazione.

{comunicazione.Testo}";
                        }
                        mail.Body = body;

                        try
                        {
                            await smtpClient.SendMailAsync(mail);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Errore invio email: " + ex.Message);
                        }
                    }
                }
            }
        }

        //----------------------------------------------//

        //metodo per il return conservando i dati proveneirnti da altri progetti via url

        private IActionResult RedirectToList()
        {
            string r = HttpContext.Session.GetString("r");
            string cod = HttpContext.Session.GetString("cod");
            string mat = HttpContext.Session.GetString("mat");
            string a = HttpContext.Session.GetString("a");
            string usr = HttpContext.Session.GetString("usr");

            if (r == "s")
            {
                return RedirectToAction("List", "Comunicazioni", new { r, cod, mat, a });
            }
            else
            {
                return RedirectToAction("List", "Comunicazioni", new { r, cod, usr });
            }
        }


        [HttpGet] //visualizzo i dati
        public IActionResult Add()
        {
            PopolaStudenti();
            PopolaDocenti();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(ListAndAddViewModel listAndAddViewModel)
        {
            var viewModel = listAndAddViewModel.AddComunicazione;
            string ruolo = HttpContext.Session.GetString("r");

            var comunicazione = new Comunicazione
            {
                Codice_Comunicazione = Guid.NewGuid(),
                DataOraComunicazione = DateTime.Now,
                Testo = viewModel.Testo.Trim(),
                K_Studente = viewModel.K_Studente,
                K_Docente = viewModel.K_Docente
            };

            if (ruolo == "a")
            {
                comunicazione.K_Soggetto = Guid.Parse(HttpContext.Session.GetString("cod"));
                comunicazione.Soggetto = "A";
                //aggiunta necessaria: di default Guid? = 00000000-0000-0000-0000-0000-0000-0000, quindi non risultava null,
                // e il meccanismo di list si inceppava

                if (comunicazione.K_Studente != null)
                {
                    comunicazione.K_Docente = null;
                }
                else if (comunicazione.K_Docente != null)
                {
                    comunicazione.K_Studente = null;
                }
            }
            else if (ruolo == "d")
            {
                comunicazione.K_Soggetto = Guid.Parse(HttpContext.Session.GetString("cod"));
                comunicazione.K_Docente = null;
                comunicazione.Soggetto = "D";
                if (viewModel.K_Studente == null || viewModel.K_Studente == Guid.Empty)  // Se "Amministrazione"
                {
                    comunicazione.K_Studente = null;  // Non associato a uno studente
                }
            }
            else if (ruolo == "s")
            {
                comunicazione.K_Soggetto = Guid.Parse(HttpContext.Session.GetString("cod"));
                comunicazione.K_Studente = null;
                comunicazione.Soggetto = "S";
                if (viewModel.K_Docente == null || viewModel.K_Docente == Guid.Empty)  // Se "Amministrazione"
                {
                    comunicazione.K_Docente = null;  // Non associato a uno studente
                }
            }

            //collegamento tra comunicazioni e studente-docente
            comunicazione.Studente = dbContext.Studenti.FirstOrDefault(s => s.K_Studente == comunicazione.K_Studente); //una join
            comunicazione.Docente = dbContext.Docenti.FirstOrDefault(d => d.K_Docente == comunicazione.K_Docente); //una join

            await dbContext.Comunicazioni.AddAsync(comunicazione);
            await dbContext.SaveChangesAsync();

            await InvioEmail(comunicazione, ruolo, "Nuova comunicazione");

            return RedirectToList();

        }

        [HttpPost]
        public async Task<IActionResult> AddRisposta(Comunicazione viewModel)
        {
            string ruolo = HttpContext.Session.GetString("r");
            Guid chiaveUtente;

            if (ruolo == "s")
            {
                chiaveUtente = Guid.Parse(HttpContext.Session.GetString("cod"));
            }
            else if (ruolo == "d")
            {
                chiaveUtente = Guid.Parse(HttpContext.Session.GetString("cod"));
            }
            else
            {
                chiaveUtente = Guid.Parse(HttpContext.Session.GetString("cod"));
            }

            var ultimaComunicazione = dbContext.Comunicazioni
                .Where(c => c.Codice_Comunicazione == viewModel.Codice_Comunicazione)
                .OrderByDescending(c => c.DataOraComunicazione)
                .FirstOrDefault();

            if (ultimaComunicazione == null)
            {
                return BadRequest("Comunicazione non trovata.");
            }

            var nuovaRisposta = new Comunicazione
            {
                Codice_Comunicazione = viewModel.Codice_Comunicazione,
                DataOraComunicazione = DateTime.Now,
                Testo = viewModel.Testo.Trim(),
                K_Soggetto = chiaveUtente,
            };



            if (ruolo == "a")
            {
                nuovaRisposta.Soggetto = "A";

                if (ultimaComunicazione.K_Soggetto != null)
                {
                    var studente = dbContext.Studenti.FirstOrDefault(s => s.K_Studente == ultimaComunicazione.K_Soggetto);
                    if (studente != null)
                    {
                        nuovaRisposta.K_Studente = studente.K_Studente;
                    }
                    else
                    {
                        // Se K_Soggetto non corrisponde a uno studente, verifica se corrisponde a un docente
                        var docente = dbContext.Docenti.FirstOrDefault(d => d.K_Docente == ultimaComunicazione.K_Soggetto);
                        if (docente != null)
                        {
                            nuovaRisposta.K_Docente = docente.K_Docente;
                        }
                    }
                }
            }

            else if (ruolo == "d")
            {
                nuovaRisposta.Soggetto = "D";
                nuovaRisposta.K_Studente = ultimaComunicazione.K_Studente;
            }
            else if (ruolo == "s")
            {
                nuovaRisposta.Soggetto = "S";
                nuovaRisposta.K_Docente = ultimaComunicazione.K_Docente;
            }

            // Assegna chi riceverà la risposta
            nuovaRisposta.Docente = dbContext.Docenti.FirstOrDefault(d => d.K_Docente == nuovaRisposta.K_Docente);
            nuovaRisposta.Studente = dbContext.Studenti.FirstOrDefault(s => s.K_Studente == nuovaRisposta.K_Studente);

            await dbContext.Comunicazioni.AddAsync(nuovaRisposta);
            await dbContext.SaveChangesAsync();

            await InvioEmail(nuovaRisposta, ruolo, "Nuova risposta alla tua comunicazione");

            return RedirectToList();
        }
    }
}

