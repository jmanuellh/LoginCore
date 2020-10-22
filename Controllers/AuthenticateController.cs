using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LoginCore.Models;
using LoginCore.Models.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LoginCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            _configuration = configuration;
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
    
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Email);

            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {

                var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };
                
                var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));

                var token = new JwtSecurityToken(
                    // issuer: _configuration["JWT:ValidIssuer"],
                    // audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature)
                );

                return Ok(new 
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            return Unauthorized();
        }
    }
}