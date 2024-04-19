namespace Dominio
{
    public class Notificacion
    {
        public static int LastId { get; set; }
        public int Id { get; set; }
        public int User {  get; set; } //Usuario al que pertenece la notificación
        public string Autor {  get; set; } //Persona de la que viene la notificación
        public Proyecto Proyecto { get; set; }
        public string Tipo { get; set; } //Ayuda, Consejo o Archivo subido a tu proyecto
        public string Estado { get; set; } //Por leer, Leídas

        public Notificacion()
        {
            Id = LastId;
            LastId++;
        }
        public Notificacion(int user, string autor, Proyecto proyecto, string tipo, string estado)
        { 
            Id = LastId;
            LastId++;
            User = user;
            Autor = autor;
            Proyecto = proyecto;
            Tipo = tipo;
            Estado = estado;
        }
    }
}
