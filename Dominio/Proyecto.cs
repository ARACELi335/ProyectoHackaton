
namespace Dominio
{
    public class Proyecto
    {
        public static int LastId { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Autor {  get; set; }
        public string Estado { get; set; } //En progreso, En pausa, Finalizado
        public string Descripcion { get; set; }
        public List<Aptitud> Tecnologias { get; set; } = new List<Aptitud>(); //Tecnologías utilizadas para el proyecto
        public List<Archivo> Archivos { get; set; } = new List<Archivo>();
        public List<Consejo> Consejos { get; set; } = new List<Consejo>();
        public List<Usuario> Ayudantes { get; set; } = new List<Usuario>(); //Usuarios que aportaron archivos al proyecto
        public List<Usuario> Consejeros { get; set; } = new List<Usuario>(); //Usuarios que aportaron consejos al proyecto
        public string Tipo { get; set; } //Público o privado

        public Proyecto() { 
            Id = LastId;
            LastId++;
        }

        public Proyecto(string nombre, string autor, string estado, string descripcion, List<Aptitud> tecnologias, string tipo)
        {
            Id = LastId;
            LastId++;
            Nombre = nombre;
            Autor = autor;
            Estado = estado;
            Descripcion = descripcion;
            Tecnologias = tecnologias;
            Tipo = tipo;
        }

        public void Validar()
        {
            if (Nombre == null | Descripcion == null | Tecnologias.Count == 0)
            {
                throw new Exception("Todos los campos deben ser completados.");
            }
        }
    }
}
