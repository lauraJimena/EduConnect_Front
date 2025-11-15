using EduConnect_Front.Dtos;
using EduConnect_Front.Services;
using EduConnect_Front.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System.Reflection;




namespace EduConnect_Front.Controllers
{
    public class AdministradorController : Controller
    {
        private const string ConsultarUsuariosAction = "ConsultarUsuarios";
        private const string ControladorAdministrador = "Administrador";
        public const string Error = "Error";
        public const string IniciarSesion = "IniciarSesion";
        public const string General = "General";
        public const string Token = "Token";
        private readonly TutoradoService _tutoradoService = new TutoradoService();
        private readonly AdministradorService _administradorService = new AdministradorService();
        private readonly GeneralService _generalService = new GeneralService();


        [HttpGet]
        [ValidarRol(3)]
        public async Task<IActionResult> RegistrarUsuariosAsync()
        {

            // Estado 1 por defecto (activo)
            var model = new CrearUsuarioDto { IdEstado = 1 };
            var carreras = await _administradorService.ObtenerCarrerasAsync();
            ViewBag.Carreras = carreras;
            return View(model);
        }

        [HttpPost]
        [ValidarRol(3)]
        public async Task<IActionResult> RegistrarUsuarios(CrearUsuarioDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                // Si los datos no son válidos, volvemos a mostrar la vista con los errores
                return View(dto);
            }

            var token = HttpContext.Session.GetString(Token);
            var (ok, msg) = await _administradorService.RegistrarUsuario(dto, token, ct);

            if (ok)
            {
                TempData["AdminRegisterOk"] = msg; // para popup de éxito

                return RedirectToAction("PanelAdministrador", ControladorAdministrador);
            }

            // Deja los datos en pantalla y muestra el error en popup
            ModelState.AddModelError(string.Empty, msg);
            var carreras = await _administradorService.ObtenerCarrerasAsync();
            ViewBag.Carreras = carreras;
            return View(dto);
        }

        [ValidarRol(3)]
        public async Task<IActionResult> PanelAdministrador()
        {
            {
                var token = HttpContext.Session.GetString(Token);

                if (string.IsNullOrEmpty(token))
                {
                    TempData[Error] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction(IniciarSesion, General);
                }
                var idUsuario = HttpContext.Session.GetInt32("IdUsu");

                if (idUsuario == null)
                {
                    TempData[Error] = "No se encontró información del usuario actual.";
                    return RedirectToAction(IniciarSesion, General);
                }
                //Volver a consultar al backend por los datos actualizados
                var usuario = await _tutoradoService.ObtenerUsuarioParaEditarAsync(idUsuario.Value, token);

                if (usuario == null)
                {
                    TempData[Error] = "No se pudo cargar el perfil del tutorado.";
                    return RedirectToAction(IniciarSesion, General);
                }

                return View(usuario);


            }
        }


        [HttpGet]
        [ValidarRol(3)]
        public async Task<IActionResult> EditarUsuario(int id)
        {
            if (!ModelState.IsValid)
            {
                TempData[Error] = "Solicitud no válida.";
                return RedirectToAction(ConsultarUsuariosAction, ControladorAdministrador);
            }
            if (id <= 0)
            {
                TempData[Error] = "Identificador de usuario no válido.";
                return RedirectToAction(ConsultarUsuariosAction, ControladorAdministrador);
            }
            try
            {
                var token = HttpContext.Session.GetString(Token);

                if (string.IsNullOrEmpty(token))
                {
                    TempData[Error] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction(IniciarSesion, General);
                }

                var (ok, msg, usuario) = await _administradorService.ObtenerUsuarioPorIdPerfil(id, token);

                if (!ok || usuario == null)
                {
                    TempData[Error] = msg ?? "Usuario no encontrado.";
                    return RedirectToAction(ConsultarUsuariosAction);
                }

                var tipoIdent = await new GeneralService().ObtenerTipoIdentAsync();
                var carreras = await _administradorService.ObtenerCarrerasAsync();

                ViewBag.TipoIdent = tipoIdent;
                ViewBag.Carreras = carreras;

                // ✅ No tocar TempData aquí
                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData[Error] = ex.Message;
                return RedirectToAction("ConsultarUsuarios");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(3)]
        public async Task<IActionResult> EditarUsuario(ActualizarUsuarioDto perfil)
        {
            if (!ModelState.IsValid)
            {
                // Si los datos no son válidos, volvemos a mostrar la vista con los errores
                return View(perfil);
            }
            try
            {
                var token = HttpContext.Session.GetString(Token);
                if (string.IsNullOrEmpty(token))
                {
                    TempData[Error] = "No se encontró el token de autenticación. Inicia sesión nuevamente.";
                    return RedirectToAction(IniciarSesion, General);
                }
                
                var (ok, msg) = await _administradorService.ActualizarUsuarioAsync(perfil, token);

                if (ok)
                    TempData["Success"] = msg;
                else
                    TempData[Error] = msg;

                // Desestructuramos la tupla que devuelve el método
                var (okUsuario, msgUsuario, usuarioDto) = await _administradorService.ObtenerUsuarioPorIdPerfil(perfil.IdUsu, token);

                ViewBag.TipoIdent = await _generalService.ObtenerTipoIdentAsync();
                ViewBag.Carreras = await _administradorService.ObtenerCarrerasAsync();

                HttpContext.Session.SetString("AvatarUrl", perfil.Avatar);
                HttpContext.Session.SetString("UsuarioNombre", perfil.Nombre);
             
                return View(usuarioDto);
            }
            catch (Exception ex)
            {
                TempData[Error] = ex.Message;
                ViewBag.TipoIdent = await _generalService.ObtenerTipoIdentAsync();
                ViewBag.Carreras = await _administradorService.ObtenerCarrerasAsync();

                return View(perfil);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> EliminarUsuario(int idUsuario,CancellationToken ct) {

            if (!ModelState.IsValid)
            {
                // Si los datos no son válidos, volvemos a mostrar la vista con los errores
                return View(idUsuario);
            }
            var token = HttpContext.Session.GetString(Token);
            if (string.IsNullOrEmpty(token))
            {
                TempData[Error] = "No se encontró token de sesión. Inicia sesión nuevamente.";
                return RedirectToAction(IniciarSesion, General);
            }
            var(ok, msg) = await _administradorService.EliminarUsuarioAsync(idUsuario, token, ct);


            if (ok)
                return Ok(msg);
            else
                return BadRequest(msg);
        }

        [HttpGet]
        [ValidarRol(3)]
        public async Task<IActionResult> ConsultarUsuarios(int? idRol, int? idEstado, string? numIdent, int pagina = 1)
        {
            if (!ModelState.IsValid)
            {
                // Si los datos no son válidos, volvemos a mostrar la vista con los errores
                return View();
            }
            const int TAMANO_PAGINA = 8;
            var token = HttpContext.Session.GetString(Token);
            if (string.IsNullOrEmpty(token))
            {
                TempData[Error] = "No se encontró token de sesión. Inicia sesión nuevamente.";
                return RedirectToAction(IniciarSesion, General);
            }

            var (ok, msg, usuarios) = await _administradorService.ObtenerUsuariosAsync(token, idRol, idEstado, numIdent);

            if (!ok)
            {
                TempData[Error] = msg;
                return View(new List<ListadoUsuariosDto>());
            }
            // 🔹 Calcular total de páginas
            var totalUsuarios = usuarios?.Count ?? 0;

            var totalPaginas = (int)Math.Ceiling((double)totalUsuarios / TAMANO_PAGINA);

            var usuariosPaginados = (usuarios ?? Enumerable.Empty<ListadoUsuariosDto>())
            .Skip((pagina - 1) * TAMANO_PAGINA)
            .Take(TAMANO_PAGINA)
            .ToList();


            // 🔹 Enviar datos a la vista
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.FiltroRol = idRol;
            ViewBag.FiltroEstado = idEstado;
            ViewBag.FiltroNumIdent = numIdent;

            return View(usuariosPaginados);

            
        }

    
        [HttpGet]
        [ValidarRol(3)]
        public async Task<IActionResult> ReporteTutoresPdf()
        {
            var token = HttpContext.Session.GetString(Token);
            if (string.IsNullOrEmpty(token))
                return RedirectToAction(IniciarSesion, General);

            var (ok, msg, reporte) = await _administradorService.ObtenerReporteTutoresAsync(token);
            if (!ok || reporte == null || reporte.Count == 0)
            {
                TempData[Error] = msg ?? "No hay datos para el reporte.";
                return RedirectToAction("PanelAdministrador", ControladorAdministrador); // redirige al panel de control
            }

            //Generar PDF 
            return new ViewAsPdf("ReporteTutoresPdf", reporte)
            {
                FileName = $"Reporte_Tutores_{DateTime.Now:yyyyMMdd}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                CustomSwitches = "--footer-center \"Página [page] de [toPage]\" --footer-font-size 9"
            };
        }

        [HttpGet]
        [ValidarRol(3)]
        public async Task<ActionResult> ReporteTutoradosPdfAsync()
        {
            var token = HttpContext.Session.GetString(Token);
            if (string.IsNullOrEmpty(token))
            {
                TempData[Error] = "Sesión expirada. Inicia sesión nuevamente.";
                return RedirectToAction(IniciarSesion, General);
            }

            var reporte = await _administradorService.ObtenerReporteTutoradosActivosAsync(token);

            if (reporte == null || reporte.Count == 0)
            {
                TempData[Error] = "No hay datos para generar el reporte.";
                return RedirectToAction("PanelAdministrador", ControladorAdministrador);
            }
            
            return new ViewAsPdf("ReporteTutoradosPdf", reporte)
            {
                FileName = $"ReporteTutorados_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }

    }
}
