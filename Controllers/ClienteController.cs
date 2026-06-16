using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using faturamentoClienteDotnet.Helpers;
using faturamentoClienteDotnet.Services;
using faturamentoClienteDotnet.Models;

using System.Security.Claims;

using System.Text.RegularExpressions;

namespace faturamentoClienteDotnet.Controllers;

[ApiController]
[Route("api/cliente")]
public class ClienteController : BaseController
{
    private readonly ClienteService _clienteService;
    private readonly JwtService _jwtService;


    public ClienteController(ClienteService clienteService, JwtService jwtService)
    {
        _clienteService = clienteService;
        _jwtService     = jwtService;
    }

    [Authorize]
    [HttpPost("cadastro_cliente")]
    public IActionResult CadastroCliente([FromBody] ClienteModel model)
    {
       if(!ModelState.IsValid){
            return BadRequest(
                ResponsesHelper.BadRequest("Dados informados inválidos", ModelState)
            );
       }

       var clienteExiste = _clienteService.GetClienteByEmail(UsuarioId, model.Email);

       if(clienteExiste is not null){
            return Conflict(
                ResponsesHelper.Conflict("Já existe um cliente com este e-mail.")
            );
       }

       var cliente = new ClienteModel {
          UsuarioId  = UsuarioId, 
          Nome       = model.Nome,
          Email      = model.Email,
          Documento  = Regex.Replace(model.Documento, @"\D", "")
       };

       var clienteId = _clienteService.SalvarCliente(cliente);

       return Ok(
         ResponsesHelper.Success( 
                "Cliente cadastrado",
                new {
                    clienteId,
                }
         )
       );
    } 

    [Authorize]
    [HttpPost("atualizar_cliente")]
    public IActionResult AtualizarCliente([FromBody] ClienteModel model)
    {
       if(!ModelState.IsValid){
            return BadRequest(
                ResponsesHelper.BadRequest("Dados informados inválidos", ModelState)
            );
       }


       var clienteExiste = _clienteService.GetClienteById(UsuarioId, model.Id);

       if(clienteExiste is null){
            return NotFound(
                ResponsesHelper.NotFound("Usuário não encontrado para atualização.")
            );
       }

        var cliente = new ClienteModel {
          Id         = model.Id, 
          Nome       = model.Nome,
          Email      = model.Email,
          Documento  = Regex.Replace(model.Documento, @"\D", "")
        };

        _clienteService.AtualizarCliente(cliente);

        return Ok(
            ResponsesHelper.Success( 
                "Usuário atualizado com sucesso."
            )
        );
    }


    [Authorize]
    [HttpPost("excluir_cliente/{clienteId}")]
    public IActionResult ExcluirCliente(int clienteId)
    {
        var clienteExiste = _clienteService.GetClienteById(UsuarioId, clienteId);

        if(clienteExiste is null){
            return NotFound(
                ResponsesHelper.NotFound("Usuário não encontrado.")
            );
        }

        _clienteService.ExcluirCliente(clienteExiste.Id);

        return Ok(
            ResponsesHelper.Success( 
                "Usuário excluído com sucesso."
            )
        );
    }

    [Authorize]
    [HttpGet("lista_clientes")]
    public IActionResult ListaCliente([FromQuery] string? nome, [FromQuery] string? email, [FromQuery] string? documento)
    {
        var clientes = _clienteService.GetClientes(UsuarioId, nome, email, documento);

        return Ok(
            ResponsesHelper.Success( 
                "Lista de clientes", 
                new {
                    clientes,
                }
            )
        );
    }
}