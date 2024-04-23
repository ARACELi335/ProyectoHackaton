namespace Dominio
{
    public class Consejo
    {
        public Usuario Autor {  get; set; }
        public string Mensaje { get; set; }

        public Consejo() { }
        public Consejo(Usuario autor, string mensaje)
        {
            Autor = autor;
            Mensaje = mensaje;
        }
    }
}
