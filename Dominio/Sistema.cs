using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Sistema
    {
        private List<Usuario> usuarios { get; set; } = new List<Usuario>();
        private List<Proyecto> proyectos { get; set; } = new List<Proyecto>();

        //Patrón Singleton
        private static Sistema instance = null;
        private Sistema() { Precarga();}
        public static Sistema GetSistema()
        {
            if (instance == null)
            {
                instance = new Sistema();
            }
            return instance;
        }

        //Precarga de datos, No es una aplicación oficial, por lo tanto no contiene datos reales.
        private void Precarga()
        {
            CreateUsuario(new Usuario("Erika", "Araceli1", "Junior"));
            GetUsuarioById(0).Aptitudes.Add(new Aptitud("java"));
            GetUsuarioById(0).Aptitudes.Add(new Aptitud("javascript"));
            GetUsuarioById(0).Aptitudes.Add(new Aptitud("c#"));
            GetUsuarioById(0).Aptitudes.Add(new Aptitud("css"));
            GetUsuarioById(0).Aptitudes.Add(new Aptitud("html"));
        }

        public void CreateUsuario(Usuario u)
        {
            if (usuarios != null)
            {
                foreach (Usuario usuario in usuarios)
                {
                    if (u.Nombre.Equals(usuario.Nombre))
                    {
                        throw new Exception("Este usuario ya se encuentra registrado.");
                    }
                }
            }
            try
            {
                u.Validar();
                usuarios.Add(u);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Usuario Login(string nombre, string password)
        {
            if (usuarios == null) { return null; }
            foreach (Usuario usuario in usuarios)
            {
                if (usuario.Nombre.Equals(nombre) && usuario.Password.Equals(password))
                {
                    return usuario;
                }
            }
            return null;
        }

        public Usuario GetUsuarioById(int? id)
        {
            if(usuarios != null)
            {
                foreach(Usuario usuario in usuarios)
                {
                    if(usuario.Id == id)
                    {
                        return usuario;
                    }
                }
            }
            return null;
        }
    }
}
