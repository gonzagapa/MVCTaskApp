using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;

namespace TareasMVC.Data
{
    public class ApplicationDbContext: DbContext
    {
        //DbContextOptions es una clase que contiene las opciones de configuracion
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        //entidades
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Paso> Pasos { get; set; }
        public DbSet<ArchivoAdjuntos> ArchivosAdjuntos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


        }
    }
}
