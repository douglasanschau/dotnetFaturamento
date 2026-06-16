using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace faturamentoClienteDotnet.Models;

public class ComprovantePagamentoModel
{
    [Required(ErrorMessage = "O arquivo deve ser informado")]
    public IFormFile Arquivo { get; set; } = null!;
}