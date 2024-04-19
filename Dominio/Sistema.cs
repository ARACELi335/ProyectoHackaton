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
        private List<Aptitud> aptitudes { get; set; } = new List<Aptitud>();

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
            //Cargar aptitudes en el sistema
            aptitudes.Add(new Aptitud("java"));
            aptitudes.Add(new Aptitud("javascript"));
            aptitudes.Add(new Aptitud("c#"));
            aptitudes.Add(new Aptitud("css"));
            aptitudes.Add(new Aptitud("sql"));
            aptitudes.Add(new Aptitud("html"));
            aptitudes.Add(new Aptitud("python"));
            //Cargar usuarios en el sistema
            CreateUsuario(new Usuario("Erika", "Araceli1", "Junior"));
            GetUsuarioById(0).Aptitudes.Add(new Aptitud("java"));
            GetUsuarioById(0).Aptitudes.Add(new Aptitud("javascript"));
            GetUsuarioById(0).Aptitudes.Add(new Aptitud("c#"));
            GetUsuarioById(0).Aptitudes.Add(new Aptitud("css"));
            GetUsuarioById(0).Aptitudes.Add(new Aptitud("html"));
            //Cargar proyectos en el sistema
            List<Aptitud> apt = new List<Aptitud>();
            apt.Add(new Aptitud("javascript"));
            apt.Add(new Aptitud("css"));
            apt.Add(new Aptitud("html"));
            Proyecto p = new Proyecto("Prueba", GetUsuarioById(0).Nombre, "En progreso", "Este proyecto es una prueba para practicar mis habilidades", apt, "Público");
            GetUsuarioById(0).MisProyectos.Add(p);
            proyectos.Add(p);
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

        public void CreateProyecto(Proyecto p)
        {
            try
            {
                p.Validar();
                
                proyectos.Add(p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AgregarAptitud(string aptitud)
        {
            bool existeAptitud = false;
            foreach (Aptitud a in aptitudes)
            {
                if (a.Nombre.Equals(aptitud))
                {
                    existeAptitud = true;
                    break;
                }
            }
            if (!existeAptitud)
            {
                aptitudes.Add(new Aptitud(aptitud));
            }
        }

        public List<Aptitud> GetListAptitudes()
        {
            return aptitudes;
        }

        public List<Proyecto> GetListProyectos()
        {
            return proyectos;
        }

        public Archivo GetArchivoById(int id)
        {
            Archivo archivo = new Archivo();
            foreach(Proyecto p in proyectos)
            {
                archivo = p.Archivos.FirstOrDefault(a => a.Id == id);
            }
            return archivo;
        }

        public List<Usuario> GetListUsuarios()
        {
            return usuarios;
        }
    }
}
