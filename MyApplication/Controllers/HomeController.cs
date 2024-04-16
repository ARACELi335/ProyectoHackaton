using Dominio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApplication.Models;
using System.Diagnostics;

namespace MyApplication.Controllers
{
    public class HomeController : Controller
    {
        Sistema s = Sistema.GetSistema();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
            if(HttpContext.Session.GetInt32("LogueadoId") != null)
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
            if(HttpContext.Session.GetInt32("LogueadoId") == null)
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
                if(aptitud == null) { throw new Exception("Introduce una aptitud."); }
                aptitud = aptitud.ToLower();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
