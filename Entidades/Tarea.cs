using System.ComponentModel.DataAnnotations;

namespace TareasMVC.Entidades
{
    public class Tarea
    {
        //Automaticamente EF Core detecta que es una llave primaria por su nomenclatura
        public int Id { get; set; }

        [StringLength(250,ErrorMessage = "El campo {0} debe tener máximo {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
        public DateTime FechaCreacion { get; set; }

        //Relacion de una tarea con muchos pasos
        public List<Paso> Pasos { get; set; }
        public List<ArchivoAdjuntos> ArchivosAdjuntos { get; set; }
    }
}
