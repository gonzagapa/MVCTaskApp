using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ClientModel.Primitives;
using TareasMVC.Models;

namespace TareasMVC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public UsuariosController(
            UserManager<IdentityUser>userManager, 
            SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager; 
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
        public IActionResult Login()
        {
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
    }
}
