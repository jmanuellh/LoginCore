using System.ComponentModel.DataAnnotations;

namespace LoginCore.Models.Authentication
{
    public class RegisterModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Correo electrónico es requerido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Contraseña es requerida")]
        public string Password { get; set; }
    }
}