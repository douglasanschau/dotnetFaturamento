using System.ComponentModel.DataAnnotations;

namespace faturamentoClienteDotnet.Models;

public class CadastroModel 
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Nome { get; set; } = "";

    [Required(ErrorMessage = "O e-mail é obrigatório")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "A senha é obrigatória")]
    public string Senha { get; set; } = "";

    [Compare("Senha", ErrorMessage = "A senha e confirmar senha devem ser iguais.")]
    public string? ConfirmarSenha { get; set; } = "";
}