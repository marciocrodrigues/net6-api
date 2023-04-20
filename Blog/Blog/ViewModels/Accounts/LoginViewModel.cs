using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Accounts
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "O E-mail é inválido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Este campo deve conter entre 3 e 10 caracteres")]
        public string Password { get; set; }
    }
}
