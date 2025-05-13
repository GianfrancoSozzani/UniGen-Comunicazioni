using Microsoft.EntityFrameworkCore;
using Comunicazioni.Models.Entities;
namespace Comunicazioni.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Comunicazione> Comunicazioni { get; set; }
        public DbSet<Studente> Studenti { get; set; }
        public DbSet<Docente> Docenti { get; set; }
        public DbSet<Corso> Corsi { get; set; }
        public DbSet<Facolta> Facolta { get; set; }
        public DbSet<TipiCorsi> TipiCorsi { get; set; }
        public DbSet<Esame> Esami { get; set; }
        public DbSet<Operatore> Operatori { get; set; }
        public DbSet<PianoStudioPersonale> PianiStudioPersonali { get; set; }
    }
}
