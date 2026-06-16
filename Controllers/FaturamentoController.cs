using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using faturamentoClienteDotnet.Helpers;
using faturamentoClienteDotnet.Services;
using faturamentoClienteDotnet.Models;
using faturamentoClienteDotnet.Enums;

using System.Security.Claims;

using System.Text.RegularExpressions;

namespace faturamentoClienteDotnet.Controllers;

[ApiController]
[Route("api/faturamento")]
public class FaturamentoController : BaseController
{
    private readonly FaturamentoService _faturamentoService;
    private readonly ClienteService _clienteService;
    private readonly JwtService _jwtService;

    public FaturamentoController(FaturamentoService faturamentoService, ClienteService clienteService, JwtService jwtService)
    {
        _faturamentoService = faturamentoService;
        _clienteService = clienteService;
        _jwtService     = jwtService;

    }

    [Authorize]
    [HttpPost("salvar_faturamento")]
    public IActionResult SalvarFaturamento([FromBody] FaturamentoModel model)
    {
        if(!ModelState.IsValid){
            return BadRequest(
                ResponsesHelper.BadRequest("Dados informados inválidos", ModelState)
            );
        }

        var clienteExiste = _clienteService.GetClienteById(UsuarioId, model.ClienteId);

        if(clienteExiste is null){
            return NotFound(
                ResponsesHelper.NotFound("Não foi possível gerar este faturamento, cliente não encontrado.")
            );
        }

        var faturamento = new FaturamentoModel {
            UsuarioId = UsuarioId,
            ClienteId = clienteExiste.Id, 
            Titulo    = model.Titulo,
            Descricao = model.Descricao,
            Valor     = model.Valor, 
            Parcelas  = model.Parcelas, 
            Status    = StatusFaturamento.Pendente
        };


        int faturamentoId = _faturamentoService.SalvarFaturamento(faturamento);

        faturamento.Id = faturamentoId;

        SalvarParcelasFaturamento(faturamento);
        

        return Ok(
            ResponsesHelper.Success( 
                "Faturamento salvo", 
                new {
                    faturamento,
                }
            )
        );
    }

    private void SalvarParcelasFaturamento(FaturamentoModel faturamento)
    {
        decimal valorParcela = faturamento.Parcelas > 1 ? Math.Round((faturamento.Valor / faturamento.Parcelas), 2) : faturamento.Valor;

        var parcelas = new List<ParcelamentoModel>();

        for(int i = 1; i <= faturamento.Parcelas; i++){
            parcelas.Add(
                new ParcelamentoModel {
                    FaturamentoId = faturamento.Id,
                    Valor         = valorParcela,
                    Status        = StatusFaturamento.Pendente
                }
            );
        }

        _faturamentoService.SalvarParcelamento(parcelas);
    }

    [Authorize]
    [HttpPost("editar_status_parcelamento")]
    public IActionResult EditarStatusParcelamento([FromBody] ParcelamentoModel model)
    {
        var parcelamento = _faturamentoService.GetParcelamentoById(UsuarioId, model.Id);

        if(parcelamento is null){
            return NotFound(
                ResponsesHelper.NotFound("Não foi possível atualizar o status, parcelamento não encontrado.")
            );
        }

        if(model.Status == 0){
            return UnprocessableEntity(
                ResponsesHelper.UnprocessableEntity("Informe o status a ser atualizado.")
            );
        }

        _faturamentoService.AtualizarStatusParcelamento(parcelamento.Id, model.Status);

        var faturamento = _faturamentoService.GetFaturamentoById(UsuarioId, parcelamento.FaturamentoId);

        if(faturamento is not null){
            AtualizarStatusFaturamento(faturamento, parcelamento);
        }

        return Ok(
            ResponsesHelper.Success( 
                "Status de parcelamento atualizado com sucesso."
            )
        );
    }

    private void AtualizarStatusFaturamento(FaturamentoModel faturamento, ParcelamentoModel parcelamento)
    {
        var parcelas = _faturamentoService.GetParcelamentosByFaturamentoId(UsuarioId, parcelamento.FaturamentoId);

        bool todasPagas      = parcelas.Where(p => p.Status != StatusFaturamento.Cancelado).All(parcela => parcela.Status == StatusFaturamento.Pago);
        bool todasCanceladas = parcelas.All(parcela => parcela.Status == StatusFaturamento.Cancelado);
        decimal valorFaturamento =  parcelas.Where(p => p.Status != StatusFaturamento.Cancelado).Sum(p => p.Valor);

        StatusFaturamento status = StatusFaturamento.Pendente;

        if(todasPagas){
            status = StatusFaturamento.Pago;
        } else if(todasCanceladas){
            status = StatusFaturamento.Cancelado;
        }

        var values = new FaturamentoModel {
            Status = status,
            Valor  = valorFaturamento
        };

        _faturamentoService.AtualizarFaturamento(faturamento.Id, UsuarioId, values);
    }

    [Authorize]
    [HttpGet("get_faturamentos_cliente/{clienteId}")]
    public ActionResult GetFaturamentosCliente(int clienteId)
    {
        ClienteModel? cliente = _clienteService.GetClienteById(UsuarioId, clienteId);

        if(cliente is null){
            return NotFound(
                ResponsesHelper.NotFound("Cliente não encontrado.")
            );
        }

        List <FaturamentoModel> faturamentos = _faturamentoService.GetFaturamentoByClienteId(cliente.Id);

        return Ok(
            ResponsesHelper.Success( 
                "Faturamentos encontrados com sucesso.",
                new {
                    faturamentos
                }
            )
        );
    }

    [Authorize]
    [HttpGet("get_parcelamentos/{faturamentoId}")]
    public ActionResult GetParcelamentosFaturamento(int faturamentoId)
    {
        FaturamentoModel? faturamento = _faturamentoService.GetFaturamentoById(UsuarioId, faturamentoId);

        if(faturamento is null){
            return NotFound(
                ResponsesHelper.NotFound("Não foi encontrado o faturamento informado.")
            );
        }

        List <ParcelamentoModel> parcelas = _faturamentoService.GetParcelamentosByFaturamentoId(UsuarioId, faturamento.Id);

        return Ok(
            ResponsesHelper.Success( 
                "Parcelamentos encontrados com sucesso.",
                new {
                    parcelas
                }
            )
        );
    }

    [Authorize]
    [HttpPost("cancelar_faturamento/{faturamentoId}")]
    public ActionResult CancelarFaturamento(int faturamentoId)
    {
        FaturamentoModel? faturamento = _faturamentoService.GetFaturamentoById(UsuarioId, faturamentoId);

        if(faturamento is null){
            return NotFound(
                ResponsesHelper.NotFound("Não foi encontrado o faturamento informado.")
            );
        }

        _faturamentoService.AtualizarFaturamento(faturamento.Id, UsuarioId, new FaturamentoModel {  Status = StatusFaturamento.Cancelado });

        var parcelas = _faturamentoService.GetParcelamentosByFaturamentoId(UsuarioId, faturamento.Id);

        if(parcelas is not null && parcelas.Any()){
            List <int> ids = parcelas .Select(p => p.Id).ToList();
            _faturamentoService.AtualizarParcelamentos(ids, new ParcelamentoModel { Status =  StatusFaturamento.Cancelado});
        }

        return Ok(
            ResponsesHelper.Success(
                "Faturamento cancelado com sucesso."
            )
        );
    }

    [Authorize]
    [HttpPost("salvar_comprovante_pagamento/{parcelamentoId}")]
    public async Task<IActionResult> SalvarComprovantePagamento(int parcelamentoId, [FromForm] ComprovantePagamentoModel model)
    {
        if (model.Arquivo == null || model.Arquivo.Length == 0)
        {
            return BadRequest(
                ResponsesHelper.BadRequest("Arquivo não informado", ModelState)
            );
        }

        var parcelamento = _faturamentoService.GetParcelamentoById(UsuarioId, parcelamentoId);

        if(parcelamento is null){
            return NotFound(
                ResponsesHelper.NotFound("Parcelamento não encontrado.")
            );
        }

        var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(model.Arquivo.FileName)}";

        var pastaUploads = Path.Combine(
            Directory.GetCurrentDirectory(),
            "uploads",
            "comprovantes"
        );

        Directory.CreateDirectory(pastaUploads);

        var caminhoCompleto = Path.Combine(
            pastaUploads,
            nomeArquivo
        );

        using (var stream = new FileStream(caminhoCompleto, FileMode.Create)){
            await model.Arquivo.CopyToAsync(stream);
        }

        var ids = new List<int> { parcelamento.Id };
        _faturamentoService.AtualizarParcelamentos(ids, new ParcelamentoModel { ComprovantePagamento =  nomeArquivo });

        return Ok(
            ResponsesHelper.Success(
                "Comprovante vinculado com sucesso."
            )
        );
    }
}

