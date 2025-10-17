using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ClientModel.Primitives;
using System.Security.Claims;
using TareasMVC.Data;
using TareasMVC.Models;
using TareasMVC.Servicios;

namespace TareasMVC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> signInManager;

        public UsuariosController(
            UserManager<IdentityUser>userManager, 
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this._context = context;
            this.signInManager = signInManager;
        }
        [AllowAnonymous] //se puede acceder sin estar autenticado
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Registro(RegistroViewModel registro)
        {
            if (!ModelState.IsValid)
            {
                return View(registro);
            }

            var usuario = new IdentityUser() { Email= registro.Email,UserName= registro.Email };

            var resultado = await userManager.CreateAsync(usuario, password: registro.Password);

            if (resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home"); 
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    //Regresar el error a la vista
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
                return View(registro);
        }

        [AllowAnonymous] //se puede acceder sin estar autenticado
        public IActionResult Login(string mensaje = null)
        {
            if(mensaje is not null)
            {
                ViewData["mensaje"] = mensaje;
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model) { 
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var resultado = await signInManager.PasswordSignInAsync(model.Email,
                model.Password, model.Recuerdame, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home"); 
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña invalidos");
                return View(model); 
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home"); 
        }

        [AllowAnonymous]
        [HttpGet]
        public ChallengeResult LoginExterno(string proveedor,string urlRetorno = null)
        {
            var urlRedireccion = Url.Action("RegistrarUsuarioExterno", values: new{ urlRetorno});
            var propiedades = signInManager.ConfigureExternalAuthenticationProperties(proveedor, urlRedireccion);
            return new ChallengeResult(proveedor, propiedades); 
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegistrarUsuarioExterno(string urlRetorno = null, 
           string remoteError=null )
        {
            urlRetorno = urlRetorno ?? Url.Content("~/");
            var mensaje = "";
            if(remoteError is not null)
            {
                mensaje = $"Error del proveedor externo: {remoteError}";
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var info = await signInManager.GetExternalLoginInfoAsync(); 
            if(info is null)
            {
                mensaje = "Error cargando la informacion del proveedor externo ";
                return RedirectToAction("login", routeValues: new { mensaje });
            }
            var resultadoLoginExterno = await signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            if (resultadoLoginExterno.Succeeded)
            {
                return LocalRedirect(urlRetorno);
            }
            string email = ""; 
            if(info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            else
            {
                mensaje = "Error obteniendo el correo electronico del proveedor externo";
                return RedirectToAction("login", routeValues: new { mensaje });
            }
            var usuario = new IdentityUser() { Email = email, UserName = email }; 
            var resultadoCreacion = await userManager.CreateAsync(usuario);

            if (!resultadoCreacion.Succeeded)
            {
                mensaje = resultadoCreacion.Errors.First().Description;
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var resultadoLoginInfo = await userManager.AddLoginAsync(usuario, info);

            if(resultadoLoginInfo.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true, info.LoginProvider);
                return LocalRedirect(urlRetorno);
            }
            mensaje = "Ha acurrido un error agregando el login externo";
            return RedirectToAction("login", routeValues: new { mensaje } );

        }

        [HttpGet]
        [Authorize(Roles = Constantes.RoleAdmin)]
        public async Task<IActionResult> Listado(string mensaje = null)
        {
            var usuarios = await _context.Users.Select(u => new UsuarioViewModel
            {
                Email = u.Email
            }).ToListAsync();

            var modelo = new ListaUsuariosViewModel();
            modelo.Usuarios = usuarios;
            modelo.Mensaje = mensaje;
            return View(modelo); 
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RoleAdmin)]
        public async Task<IActionResult> HacerAdmin(string Email)
        {
            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);

            if(usuario is null)
            {
                return NotFound();
            }
            await userManager.AddToRoleAsync(usuario, Constantes.RoleAdmin);
            return RedirectToAction("Listado", routeValues: new { mensaje = "Rol asignado correctamente" });
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RoleAdmin)] // redirigir al login si no tenemos el accoes 
        public async Task<IActionResult> RemoverAdmin(string Email)
        {
            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);

            if (usuario is null)
            {
                return NotFound();
            }
            await userManager.RemoveFromRoleAsync(usuario, Constantes.RoleAdmin);
            return RedirectToAction("Listado", routeValues: new { mensaje = "Rol removido correctamente" });
        }

    }
}
