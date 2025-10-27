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
        private readonly TutoradoService _tutoradoService = new TutoradoService();
        private readonly AdministradorService _administradorService = new AdministradorService();
        private readonly GeneralService _generalService = new GeneralService();

        
        [HttpGet]
        [ValidarRol(3)]
        public IActionResult RegistrarUsuarios()
        {
            // Estado 1 por defecto (activo)
            var model = new CrearUsuarioDto { IdEstado = 1 };
            return View(model);
        }

        [HttpPost]
        [ValidarRol(3)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarUsuarios(CrearUsuarioDto dto, CancellationToken ct)
        {
            var token = dto.Token ?? string.Empty;
            token=HttpContext.Session.GetString("Token");
            var (ok, msg) = await _administradorService.RegistrarUsuario(dto,token,ct);

            if (ok)
            {
                TempData["AdminRegisterOk"] = msg; // para popup de éxito
                
                return RedirectToAction("PanelAdministrador", "Administrador");
            }

            // Deja los datos en pantalla y muestra el error en popup
            ModelState.AddModelError(string.Empty, msg);
            return View(dto);
        }

        [ValidarRol(3)]
        public async Task<IActionResult> PanelAdministrador()
        {
            {
                var token = HttpContext.Session.GetString("Token");

                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }
                var idUsuario = HttpContext.Session.GetInt32("IdUsu");

                if (idUsuario == null)
                {
                    TempData["Error"] = "No se encontró información del usuario actual.";
                    return RedirectToAction("IniciarSesion", "General");
                }
                //Volver a consultar al backend por los datos actualizados
                var usuario = await _tutoradoService.ObtenerUsuarioParaEditarAsync(idUsuario.Value, token);

                if (usuario == null)
                {
                    TempData["Error"] = "No se pudo cargar el perfil del tutorado.";
                    return RedirectToAction("IniciarSesion", "General");
                }

                return View(usuario);




            }
        }


        [HttpGet]
        [ValidarRol(3)]
        public async Task<IActionResult> EditarUsuario(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");

                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }

                var (ok, msg, usuario) = await _administradorService.ObtenerUsuarioPorIdPerfil(id, token);

                if (!ok || usuario == null)
                {
                    TempData["Error"] = msg ?? "Usuario no encontrado.";
                    return RedirectToAction("ConsultarUsuarios");
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
                TempData["Error"] = ex.Message;
                return RedirectToAction("ConsultarUsuarios");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarRol(3)]
        public async Task<IActionResult> EditarUsuario(ActualizarUsuarioDto perfil)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "No se encontró el token de autenticación. Inicia sesión nuevamente.";
                    return RedirectToAction("IniciarSesion", "General");
                }
                
                var (ok, msg) = await _administradorService.ActualizarUsuarioAsync(perfil, token);

                if (ok)
                    TempData["Success"] = msg;
                else
                    TempData["Error"] = msg;

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
                TempData["Error"] = ex.Message;
                ViewBag.TipoIdent = await _generalService.ObtenerTipoIdentAsync();
                ViewBag.Carreras = await _administradorService.ObtenerCarrerasAsync();

                return View(perfil);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> EliminarUsuario(int idUsuario,CancellationToken ct) { 


              var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "No se encontró token de sesión. Inicia sesión nuevamente.";
                return RedirectToAction("IniciarSesion", "General");
            }
            var(ok, msg) = await _administradorService.EliminarUsuarioAsync(idUsuario, token, ct);


            if (ok)
                return Ok(msg);
            else
                return BadRequest(msg);
        }

        [HttpGet]
        [ValidarRol(3)]
        public async Task<IActionResult> ConsultarUsuarios(int? idRol, int? idEstado, string? numIdent)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "No se encontró token de sesión. Inicia sesión nuevamente.";
                return RedirectToAction("IniciarSesion", "General");
            }

            var (ok, msg, usuarios) = await _administradorService.ObtenerUsuariosAsync(token, idRol, idEstado, numIdent);

            if (!ok)
            {
                TempData["Error"] = msg;
                return View(new List<ListadoUsuariosDto>());
            }

           
            return View(usuarios);
        }

    
        [HttpGet]
        [ValidarRol(3)]
        public async Task<IActionResult> ReporteTutoresPdf()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("IniciarSesion", "General");

            var (ok, msg, reporte) = await _administradorService.ObtenerReporteTutoresAsync(token);
            if (!ok || reporte == null || !reporte.Any())
            {
                TempData["Error"] = msg ?? "No hay datos para el reporte.";
                return RedirectToAction("PanelAdministrador", "Administrador"); // redirige al panel de control
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
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Sesión expirada. Inicia sesión nuevamente.";
                return RedirectToAction("IniciarSesion", "General");
            }

            var reporte = await _administradorService.ObtenerReporteTutoradosActivosAsync(token);

            if (reporte == null || !reporte.Any())
            {
                TempData["Error"] = "No hay datos para generar el reporte.";
                return RedirectToAction("PanelAdministrador", "Administrador");
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
