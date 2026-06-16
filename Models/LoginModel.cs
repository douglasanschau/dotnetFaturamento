using System.ComponentModel.DataAnnotations;

namespace faturamentoClienteDotnet.Models;

public class LoginModel 
{
    [Required(ErrorMessage = "O e-mail é obrigatório")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "A senha é obrigatória")]
    public string Senha { get; set; } = "";
}