using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dominio
{
    public class Usuario
    {
        public static int LastId { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public List<Aptitud> Aptitudes { get; set; } = new List<Aptitud>();
        public List<Proyecto> MisProyectos { get; set; } = new List<Proyecto>();
        public List<Proyecto> Proyectos { get; set; } = new List<Proyecto>(); //Proyectos de otras personas en los que el ususario ayudó
        public List<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
        public string Nivel { get; set; } //Junior, semi senior, senior

        public Usuario()
        {
            Id = LastId;
            LastId++;
        }

        public Usuario(string nombre, string password, string nivel)
        {
            Id = LastId;
            LastId++;
            Nombre = nombre;
            Password = password;
            Nivel = nivel;
        }

        public void Validar()
        {
            if (Nombre == null | Password == null) { throw new Exception("Todos los campos deben ser completados."); }
            else if (Nivel == "X") { throw new Exception("Debe seleccionar un nivel."); }
            else if (Password.Length < 8){ throw new Exception("La contraseña debe tener al menos 8 caracteres."); }
            else if (!Password.Any(char.IsUpper) | !Password.Any(char.IsLower) | !Password.Any(char.IsDigit))
            {
                throw new Exception("La contraseña debe tener al menos una letra mayúscula, una minúscula y un número");
            }
        }
    }
}
