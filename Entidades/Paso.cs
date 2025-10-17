namespace TareasMVC.Entidades
{
    public class Paso
    {
        //Identificador global unico //0ebvf300-....
        public Guid Id { get; set; }

        //Se configura como una llave foranea por su nomenclatura
        //Relacion de muchos pasos con una tarea
        public int TareaId { get; set; }

        //Propiedad de navegacion
        public Tarea Tarea { get; set; }

        public string Descripcion { get; set; }
        public bool Realizado { get; set; }
        public int Orden { get; set; }

    }
}
