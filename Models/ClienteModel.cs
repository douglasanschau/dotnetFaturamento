using System.ComponentModel.DataAnnotations;
using faturamentoClienteDotnet.Validators;

namespace faturamentoClienteDotnet.Models;

public class ClienteModel
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }


    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Nome { get; set; } = "";

    [Required(ErrorMessage = "O e-mail é obrigatório")]
    public string Email { get; set; } = "";

    [Documento]
    public string Documento { get; set; } = "";
}