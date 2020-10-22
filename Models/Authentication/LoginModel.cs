using System.ComponentModel.DataAnnotations;

namespace LoginCore.Models.Authentication
{
    public class LoginModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Correo es requerido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Contraseña es requerida")]
        public string Password { get; set; }
    }
}