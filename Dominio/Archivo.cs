using System.Globalization;

namespace Dominio
{
    public class Archivo
    {
        public static int LastId { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Contenido { get; set; }
        public string Autor {  get; set; }
        public Aptitud Lenguaje { get; set; }

        public Archivo() {
            Id = LastId;
            LastId++;
        }

        public Archivo(string nombre, string contenido, string autor, Aptitud lenguaje)
        {
            Id = LastId;
            LastId++;
            Nombre = nombre;
            Contenido = contenido;
            Autor = autor;
            Lenguaje = lenguaje;
        }
    }
}
