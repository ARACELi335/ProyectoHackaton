using Dominio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApplication.Models;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text;


namespace MyApplication.Controllers
{
    public class HomeController : Controller
    {
        Sistema s = Sistema.GetSistema();
        private IWebHostEnvironment _environment;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public IActionResult Index()
        {
            //Registrarse
            int? logueadoId = HttpContext.Session.GetInt32("LogueadoId");
            if (logueadoId == null)
            {
                return RedirectToAction("Registro");
            }
            return View();
        }

        public IActionResult Registro()
        {
            //Registrarse
            if (HttpContext.Session.GetInt32("LogueadoId") != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Registro(string nombre, string password, string nivel)
        {
            try
            {
                s.CreateUsuario(new Usuario(nombre, password, nivel));
                HttpContext.Session.SetInt32("LogueadoId", s.Login(nombre, password).Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("LogueadoId") != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string nombre, string password)
        {
            Usuario buscado = s.Login(nombre, password);
            if (buscado != null)
            {
                HttpContext.Session.SetInt32("LogueadoId", buscado.Id);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.msg = "La contraseña y el usuario no coinciden";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult MiPerfil(Usuario u)
        {
            if (HttpContext.Session.GetInt32("LogueadoId") == null)
            {
                return RedirectToAction("Registro");
            }
            else
            {
                Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
                return View(usuario);
            }

        }

        public IActionResult EditPerfil()
        {
            if (HttpContext.Session.GetInt32("LogueadoId") == null)
            {
                return RedirectToAction("Registro");
            }
            else
            {
                Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
                return View(usuario);
            }
        }

        [HttpPost]
        public IActionResult EditPerfil(Usuario editado)
        {
            Usuario viejo = s.GetUsuarioById(editado.Id);
            viejo.Nombre = editado.Nombre;
            viejo.Password = editado.Password;
            viejo.Nivel = editado.Nivel;
            return RedirectToAction("MiPerfil");
        }

        public IActionResult EditAptitud()
        {
            if (HttpContext.Session.GetInt32("LogueadoId") == null)
            {
                return RedirectToAction("Registro");
            }
            else
            {
                Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
                return View(usuario);
            }
        }

        [HttpPost]
        public IActionResult EditAptitud(int? id, string aptitud)
        {
            Usuario usuario = s.GetUsuarioById(id);
            try
            {
                if (aptitud == null) { throw new Exception("Introduce una aptitud."); }
                aptitud = aptitud.ToLower();
                s.AgregarAptitud(aptitud); //Agrega aptitudes a una lista de aptitudes general
                bool ExisteAptitud = false;
                foreach (Aptitud a in usuario.Aptitudes)
                {
                    if (a.Nombre.Equals(aptitud))
                    {
                        ExisteAptitud = true;
                    }
                }
                if (!ExisteAptitud)
                {
                    usuario.Aptitudes.Add(new Aptitud(aptitud));
                }
                else
                {
                    ViewBag.msg = "Ya tienes esta aptitud en tu perfil.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
            }
            return View(usuario);

        }

        public IActionResult DeleteAptitud(int id)
        {
            Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
            usuario.Aptitudes.Remove(usuario.Aptitudes.FirstOrDefault(a => a.Id == id));
            return RedirectToAction("EditAptitud", usuario.Id);
        }

        public IActionResult CreateProyecto()
        {
            if (HttpContext.Session.GetInt32("LogueadoId") == null)
            {
                return RedirectToAction("Registro");
            }
            else
            {
                ViewBag.userId = HttpContext.Session.GetInt32("LogueadoId");
                ViewBag.aptitudes = s.GetListAptitudes();
                return View();
            }

        }
        [HttpPost]
        public IActionResult CreateProyecto(Proyecto p, string a1, string a2, string a3, int userId)
        {
            Usuario u = s.GetUsuarioById(userId);
            HttpContext.Session.SetInt32("LogueadoId", u.Id);
            if (HttpContext.Session.GetInt32("LogueadoId") == null)
            {
                return RedirectToAction("Registro");
            }
            else
            {

                try
                {
                    foreach (Proyecto pro in u.MisProyectos)
                    {
                        if (pro.Nombre.Equals(p.Nombre))
                        {
                            throw new Exception("Ya tienes un proyecto con este nombre.");
                        }
                    }
                    p.Autor = u.Nombre;
                    p.Estado = "En progreso";
                    if (a1 == "X" && a2 == "X" && a3 == "X")
                    {
                        throw new Exception("Escoja al menos una tecnología.");
                    }
                    else
                    {
                        if (a1 != "X") { p.Tecnologias.Add(new Aptitud(a1)); }
                        if (a2 != "X" && a2 != a1) { p.Tecnologias.Add(new Aptitud(a2)); }
                        if (a3 != "X" && a3 != a1 && a3 != a2) { p.Tecnologias.Add(new Aptitud(a3)); }
                    }
                    p.Validar();
                    u.MisProyectos.Add(p);
                    s.GetListProyectos().Add(p);
                    ViewBag.msg = "El proyecto ha sido creado correctamente.";
                }
                catch (Exception ex)
                {
                    ViewBag.msg = ex.Message;
                }
                ViewBag.userId = u.Id;
                ViewBag.aptitudes = s.GetListAptitudes();
                return View();
            }
        }

        public IActionResult DetailsProyecto(Proyecto p)
        {
            if (HttpContext.Session.GetInt32("LogueadoId") == null)
            {
                return RedirectToAction("Registro");
            }
            else
            {
                ViewBag.msg = TempData["msg"];
                Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
                Proyecto proyecto = s.GetListProyectos().FirstOrDefault(pr => pr.Id == p.Id);
                HttpContext.Session.SetInt32("LogueadoId", usuario.Id);
                ViewBag.usuarioLogueado = usuario.Nombre;
                return View(proyecto);
            }
        }
        public IActionResult DeleteProyecto(int id)
        {
            Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
            usuario.MisProyectos.Remove(usuario.MisProyectos.FirstOrDefault(p => p.Id == id));
            s.GetListProyectos().Remove(s.GetListProyectos().FirstOrDefault(p => p.Id == id));
            return RedirectToAction("MiPerfil", usuario.Id);
        }
        public IActionResult PausarProyecto(string nombre)
        {
            Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
            usuario.MisProyectos.FirstOrDefault(p => p.Nombre == nombre).Estado = "En pausa";
            s.GetListProyectos().FirstOrDefault(p => p.Nombre == nombre).Estado = "En pausa";
            return RedirectToAction("DetailsProyecto", s.GetListProyectos().FirstOrDefault(p => p.Nombre == nombre));
        }
        public IActionResult ActivarProyecto(string nombre)
        {
            Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
            usuario.MisProyectos.FirstOrDefault(p => p.Nombre == nombre).Estado = "En progreso";
            s.GetListProyectos().FirstOrDefault(p => p.Nombre == nombre).Estado = "En progreso";
            return RedirectToAction("DetailsProyecto", s.GetListProyectos().FirstOrDefault(p => p.Nombre == nombre));
        }
        public IActionResult TerminarProyecto(string nombre)
        {
            Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
            usuario.MisProyectos.FirstOrDefault(p => p.Nombre == nombre).Estado = "Finalizado";
            s.GetListProyectos().FirstOrDefault(p => p.Nombre == nombre).Estado = "Finalizado";
            return RedirectToAction("DetailsProyecto", s.GetListProyectos().FirstOrDefault(p => p.Nombre == nombre));
        }
        public IActionResult ProyectoPrivado(string nombre)
        {
            Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
            usuario.MisProyectos.FirstOrDefault(p => p.Nombre == nombre).Tipo = "Privado";
            s.GetListProyectos().FirstOrDefault(p => p.Nombre == nombre).Tipo = "Privado";
            return RedirectToAction("DetailsProyecto", s.GetListProyectos().FirstOrDefault(p => p.Nombre == nombre));
        }
        public IActionResult ProyectoPublico(string nombre)
        {
            Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
            usuario.MisProyectos.FirstOrDefault(p => p.Nombre == nombre).Tipo = "Público";
            s.GetListProyectos().FirstOrDefault(p => p.Nombre == nombre).Tipo = "Público";
            return RedirectToAction("DetailsProyecto", s.GetListProyectos().FirstOrDefault(p => p.Nombre == nombre));
        }

        public IActionResult SubirArchivo(Proyecto pro)
        {
            if (HttpContext.Session.GetInt32("LogueadoId") == null)
            {
                return RedirectToAction("Registro");
            }
            else
            {
                Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
                ViewBag.usuario = usuario.Id;
                Proyecto proyecto = s.GetListProyectos().FirstOrDefault(p => p.Id == pro.Id);
                return View(proyecto);
            }
        }

        [HttpPost]
        public IActionResult SubirArchivo(int? id, string nombre, IFormFile archivo, int? idUsuario)
        {
            Usuario usuario = s.GetUsuarioById(idUsuario);
            HttpContext.Session.SetInt32("LogueadoId", usuario.Id);

            Proyecto proyecto = s.GetListProyectos().FirstOrDefault(p => p.Id == id);
            try
            {
                if (archivo == null) { throw new Exception("El archivo es obligatorio."); }
                string contenido;
                using (var reader = new StreamReader(archivo.OpenReadStream()))
                {
                    contenido = reader.ReadToEnd();
                }
                string extension = Path.GetExtension(archivo.FileName);
                if(nombre == null)
                {
                    nombre = archivo.FileName;
                }
                if (proyecto.Archivos.FirstOrDefault(a => a.Nombre == nombre) == null)
                {
                    Archivo arch = new Archivo(nombre, contenido, usuario.Nombre, new Aptitud(extension));
                    proyecto.Archivos.Add(arch);
                    ViewBag.msg = "El archivo se ha subido correctamente";
                }
                else
                {
                    throw new Exception("Ya existe un archivo con el mismo nombre en este proyecto");
                }
                if (usuario.Nombre != proyecto.Autor)
                {
                    usuario.Proyectos.Add(proyecto);
                    Usuario u = s.GetListUsuarios().FirstOrDefault(us => us.Nombre == proyecto.Autor);
                    u.Notificaciones.Add(new Notificacion(u.Id, usuario.Nombre, proyecto, "Archivo subido", "Por leer"));
                }
                
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
            }
            ViewBag.usuario = usuario.Id;
            return View(proyecto);

        }

        public IActionResult DeleteArchivo(int archivoId, int proyectoId)
        {
            Proyecto proyecto = s.GetListProyectos().FirstOrDefault(p => p.Id == proyectoId);
            proyecto.Archivos.Remove(s.GetArchivoById(archivoId));
            return RedirectToAction("DetailsProyecto", proyecto);
        }

        public IActionResult DetailsArchivo(Archivo a)
        {
            if (HttpContext.Session.GetInt32("LogueadoId") == null)
            {
                return RedirectToAction("Registro");
            }
            else
            {
                Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
                Archivo archivo = s.GetArchivoById(a.Id);
                HttpContext.Session.SetInt32("LogueadoId", usuario.Id);
                return View(archivo);
            }
        }

        public IActionResult PedirAyuda(int id)
        {
            Proyecto proyecto = s.GetListProyectos().FirstOrDefault(p => p.Id == id);
            try
            {
                foreach (Aptitud a in proyecto.Tecnologias)
                {
                    foreach (Usuario u in s.GetListUsuarios())
                    {
                        if (u.Nombre != proyecto.Autor &&
                           u.Notificaciones.FirstOrDefault(n => n.Proyecto.Equals(proyecto) && n.Tipo == "Ayuda") == null &&
                           u.Aptitudes.FirstOrDefault(ap => ap.Nombre.Equals(a.Nombre)) != null)
                        {
                            u.Notificaciones.Add(new Notificacion(u.Id, proyecto.Autor, proyecto, "Ayuda", "Por leer"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            TempData["msg"] = "La ayuda ha sido solicitada a otros usuarios.";
            return RedirectToAction("DetailsProyecto", proyecto);
        }
        public IActionResult Notificaciones(Usuario u)
        {
            if (HttpContext.Session.GetInt32("LogueadoId") == null)
            {
                return RedirectToAction("Registro");
            }
            else
            {
                Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
                return View(usuario);
            }
        }
        public IActionResult DeleteNotificacion(int notiId, int usuarioId)
        {
            Usuario usuario = s.GetUsuarioById(usuarioId);
            usuario.Notificaciones.Remove(usuario.Notificaciones.FirstOrDefault(n => n.Id == notiId));
            return RedirectToAction("Notificaciones", usuario);
        }

        public IActionResult EditTecnologia(int id)
        {
            if (HttpContext.Session.GetInt32("LogueadoId") == null)
            {
                return RedirectToAction("Registro");
            }
            else
            {
                ViewBag.msg = TempData["msg"];
                Usuario usuario = s.GetUsuarioById(HttpContext.Session.GetInt32("LogueadoId"));
                return View(usuario.MisProyectos.FirstOrDefault(p => p.Id == id));
            }
        }

        [HttpPost]
        public IActionResult EditTecnologia(int? id, string tecnologia)
        {
            Proyecto proyecto = s.GetListProyectos().FirstOrDefault(p => p.Id == id);
            try
            {
                if (tecnologia == null) { throw new Exception("Introduce una tecnología."); }
                tecnologia = tecnologia.ToLower();
                s.AgregarAptitud(tecnologia); //Agrega aptitudes a una lista de aptitudes general
                bool Existe = false;
                foreach (Aptitud a in proyecto.Tecnologias)
                {
                    if (a.Nombre.Equals(tecnologia))
                    {
                        Existe = true;
                    }
                }
                if (!Existe)
                {
                    proyecto.Tecnologias.Add(new Aptitud(tecnologia));
                }
                else
                {
                    ViewBag.msg = "Ya tienes esta tecnología en tu proyecto.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
            }
            return View(proyecto);

        }

        public IActionResult DeleteTecnologia(int id, int idProyecto)
        {
            Proyecto proyecto = s.GetListProyectos().FirstOrDefault(p => p.Id == idProyecto);
            if(proyecto.Tecnologias.Count == 1)
            {
                TempData["msg"] = "Debes tener al menos una tecnología en tu proyecto";
            }
            else
            {
                proyecto.Tecnologias.Remove(proyecto.Tecnologias.FirstOrDefault(a => a.Id == id));
            }
            
            return RedirectToAction("EditTecnologia", proyecto.Id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
