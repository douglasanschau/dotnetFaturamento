using System.ComponentModel.DataAnnotations;

using faturamentoClienteDotnet.Enums;

namespace faturamentoClienteDotnet.Models;

public class FaturamentoModel
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Cliente inválido.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "O titulo é obrigatório")]
    public string  Titulo { get; set; } = "";
    public string? Descricao { get; set; } = "";

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor é obrigatório e deve ser maior que zero.")]
    public decimal Valor { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantidade de parcelas inválida.")]
    public int Parcelas { get; set; } 
    
    public StatusFaturamento Status { get; set; }
}