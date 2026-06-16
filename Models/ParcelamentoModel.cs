using System.ComponentModel.DataAnnotations;
using faturamentoClienteDotnet.Validators;

using faturamentoClienteDotnet.Enums;

namespace faturamentoClienteDotnet.Models;

public class ParcelamentoModel
{
    public int Id { get; set; }
    public int FaturamentoId { get; set; }
    public decimal Valor { get; set; }
    public StatusFaturamento Status { get; set; }
    public string? ComprovantePagamento { get; set; }
}