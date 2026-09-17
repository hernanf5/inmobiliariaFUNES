using System;
using inmobiliariaFUNES.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace inmobiliariaFUNES.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IRepositorioUsuario repositorio;
        private readonly ServicioHash servicioHash;
        private readonly ILogger<UsuariosController> logger;

        public UsuariosController(IRepositorioUsuario repo, ServicioHash servicioHash, ILogger<UsuariosController> logger)
        {
            this.repositorio = repo;
            this.servicioHash = servicioHash;
            this.logger = logger;
        }

        // GET: Usuarios
        [Authorize(Policy = "Administrador")]
        [Route("[controller]/Index")]
        public ActionResult Index(int pagina = 1)
        {
            try
            {
                var tamaño = 10;
                var lista = repositorio.ObtenerLista(Math.Max(pagina, 1), tamaño);
                ViewBag.Pagina = pagina;
                var total = repositorio.ObtenerCantidad();
                ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
                ViewBag.Id = TempData["Id"];
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                return View(lista);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Index");
                throw;
            }
        }
        // GET: Usuarios/Details/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Details(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Details");
                throw;
            }
        }

        // GET: Usuarios/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                throw;
            }
        }
        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Usuario usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Roles = Usuario.ObtenerRoles();
                    return View(usuario);
                }

                usuario.Clave = servicioHash.Hashear(usuario.Clave);
                repositorio.Alta(usuario);
                TempData["Id"] = usuario.IdUsuario;
                TempData["Mensaje"] = "Usuario creado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                throw;
            }
        }
        // GET: Usuarios/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit");
                throw;
            }
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id, Usuario entidad)
        {
            try
            {
                ModelState.Remove(nameof(Usuario.Clave));
                var u = repositorio.ObtenerPorId(id);
                if (u == null)
                    return NotFound();

                // Ojo: nunca tocamos u.Clave 
                u.Nombre = entidad.Nombre;
                u.Email = entidad.Email;
                u.Rol = entidad.Rol;
                repositorio.Modificacion(u);

                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit");
                throw;
            }
        }
        // GET: Usuarios/CambiarClave/5
        [Authorize]
        public ActionResult CambiarClave(int id)
        {
            try
            {
                if (!EsAdminOSosVos(id))
                    return Forbid();

                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en CambiarClave");
                throw;
            }
        }

        // POST: Usuarios/CambiarClave/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CambiarClave(int id, string claveNueva)
        {
            try
            {
                if (!EsAdminOSosVos(id))
                    return Forbid();

                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();

                if (string.IsNullOrWhiteSpace(claveNueva) || claveNueva.Length < 6)
                {
                    TempData["Error"] = "La contraseña debe tener al menos 6 caracteres.";
                    return RedirectToAction(nameof(CambiarClave), new { id });
                }

                entidad.Clave = servicioHash.Hashear(claveNueva);
                repositorio.Modificacion(entidad);

                TempData["Mensaje"] = "Contraseña actualizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en CambiarClave");
                throw;
            }
        }

        private bool EsAdminOSosVos(int idUsuarioObjetivo)
        {
            if (User.IsInRole("Administrador"))
                return true;
            var idPropio = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return idPropio == idUsuarioObjetivo.ToString();
        }
        // GET: Usuarios/Eliminar/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Eliminar");
                throw;
            }
        }

        // POST: Usuarios/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id, Usuario entidad)
        {
            try
            {
                var u = repositorio.ObtenerPorId(id);
                if (u == null)
                    return NotFound();

                repositorio.Baja(u);
                TempData["Mensaje"] = "Usuario dado de baja correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Eliminar");
                throw;
            }
        }
        [AllowAnonymous]
        public ActionResult Login(string? returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView login)
        {
            try
            {
                var returnUrl = string.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/" : (TempData["returnUrl"] as string)!;

                if (!ModelState.IsValid)
                {
                    TempData["returnUrl"] = returnUrl;
                    return View(login);
                }

                var usuario = repositorio.ObtenerPorEmail(login.Email);
                var claveHasheada = servicioHash.Hashear(login.Clave);

                if (usuario == null || usuario.Clave != claveHasheada)
                {
                    ModelState.AddModelError(string.Empty, "El email o la contraseña no son correctos.");
                    TempData["returnUrl"] = returnUrl;
                    return View(login);
                }

                var claims = new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, usuario.Email),
                    new System.Security.Claims.Claim("NombreCompleto", usuario.Nombre),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, usuario.RolNombre),
                };

                var claimsIdentity = new System.Security.Claims.ClaimsIdentity(
                    claims,
                    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                    new System.Security.Claims.ClaimsPrincipal(claimsIdentity));

                TempData.Remove("returnUrl");
                return Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Login");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al iniciar sesión.");
                return View(login);
            }
        }

                // GET: Usuarios/MiPerfil
        [Authorize]
        public ActionResult MiPerfil()
        {
            try
            {
                var idPropio = ObtenerIdUsuarioLogueado();
                var entidad = repositorio.ObtenerPorId(idPropio);
                if (entidad == null)
                    return NotFound();
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en MiPerfil");
                throw;
            }
        }

        // POST: Usuarios/MiPerfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> MiPerfil(Usuario entidad)
        {
            try
            {
                ModelState.Remove(nameof(Usuario.Clave));
                ModelState.Remove(nameof(Usuario.Rol));

                var idPropio = ObtenerIdUsuarioLogueado();
                var u = repositorio.ObtenerPorId(idPropio);
                if (u == null)
                    return NotFound();

                if (!ModelState.IsValid)
                    return View(entidad);

                u.Nombre = entidad.Nombre;
                u.Email = entidad.Email;
                repositorio.Modificacion(u);

                // Refrescamos la cookie con los datos nuevos, para que el
                // navbar se actualice sin tener que cerrar sesión y volver a entrar.
                var claims = new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, u.IdUsuario.ToString()),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, u.Email),
                    new System.Security.Claims.Claim("NombreCompleto", u.Nombre),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, u.RolNombre),
                };
                var claimsIdentity = new System.Security.Claims.ClaimsIdentity(
                    claims,
                    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                    new System.Security.Claims.ClaimsPrincipal(claimsIdentity));

                TempData["Mensaje"] = "Tus datos se actualizaron correctamente";
                return RedirectToAction(nameof(MiPerfil));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en MiPerfil");
                throw;
            }
        }

        private int ObtenerIdUsuarioLogueado()
        {
            var idTexto = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idTexto, out var id) ? id : 0;
        }

        [Route("salir")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}