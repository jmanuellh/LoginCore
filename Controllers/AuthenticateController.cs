using System;
using System.Linq;
using System.Threading.Tasks;
using LoginCore.Models;
using LoginCore.Models.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LoginCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AuthenticateController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // ApplicationUser correoExiste = await userManager.FindByNameAsync(model.Email);

            // if(correoExiste != null)
            // {
            //     Console.WriteLine("entro a error de correo exite");
            //     return StatusCode(StatusCodes.Status409Conflict, "El usuario existe");
            // }

            ApplicationUser user = new ApplicationUser()
                {
                    UserName = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
            
            IdentityResult resultado = await userManager.CreateAsync(user, model.Password);

            if(!resultado.Succeeded)
            {
                // return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Error al crear el usuario, porfavor revise la información del usuario" });
                return StatusCode(StatusCodes.Status409Conflict, new Response { Status = "Error", Message = resultado.Errors.FirstOrDefault().Description });
            }
                
            return Ok(new Response { Status = "Success", Message = "Usuario creado correctamente" });
        }
    }
}