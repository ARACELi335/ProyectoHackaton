namespace Dominio
{
    public class Aptitud
    {
        public string Nombre { get; set; }
        public static int LastId { get; set; }
        public int Id { get; set; }

        public Aptitud(string nombre)
        {
            Id = LastId;
            LastId++;
            Nombre = nombre;
        }
    }
}
