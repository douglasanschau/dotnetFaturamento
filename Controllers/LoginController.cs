using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using faturamentoClienteDotnet.Models;
using faturamentoClienteDotnet.Services;
using faturamentoClienteDotnet.Helpers;
using Microsoft.AspNetCore.Identity;

namespace faturamentoClienteDotnet.Controllers;

[ApiController]
[Route("api/login")]

public class LoginController : ControllerBase
{
    private readonly UsuarioService _usuarioService;
    private readonly JwtService _jwtService;

    public LoginController(UsuarioService usuarioService, JwtService jwtService)
    {
        _usuarioService = usuarioService;
        _jwtService     = jwtService;

    }

    [HttpPost]
    public IActionResult Index([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid){
            return BadRequest(
                ResponsesHelper.BadRequest("Dados informados inválidos", ModelState)
            );
        }

        var usuario = _usuarioService.GetUsuarioByEmail(model.Email);
        var resultado = PasswordVerificationResult.Failed;

        if(usuario is not null){
            var hasher = new PasswordHasher<UsuarioModel>();

            resultado = hasher.VerifyHashedPassword(
                usuario,
                usuario.Senha,
                model.Senha
            );
        }

        if((usuario is null || (resultado != PasswordVerificationResult.Success))){
            return Unauthorized(
                ResponsesHelper.Unauthorized("Usuário ou senha inválidos")
            );
        }
        
        var token = _jwtService.GenerateToken(usuario);

        return Ok(
            ResponsesHelper.Success( 
                "Usuário encontrado",
                new 
                {
                    token,
                    usuario,
                }
            )
        );
    } 

    [HttpPost("cadastro")]
    public IActionResult Cadastro([FromBody] CadastroModel model) 
    {
        if (!ModelState.IsValid){
            return BadRequest(
                ResponsesHelper.BadRequest("Dados informados inválidos", ModelState)
            );
        }

        var usuarioExistente = _usuarioService.GetUsuarioByEmail(model.Email);

        if (usuarioExistente is not null){
            return Conflict(
                ResponsesHelper.Conflict("Já existe um usuário com este email")
            );
        }

        var hasher = new PasswordHasher<UsuarioModel>();

        var novoUsuario = new UsuarioModel {
            Nome = model.Nome,
            Email = model.Email,
        };

        novoUsuario.Senha = hasher.HashPassword(novoUsuario, model.Senha);

        var usuario_id = _usuarioService.SalvarUsuario(novoUsuario);

        var usuario = _usuarioService.GetUsuarioById(usuario_id);

        return Ok(
            ResponsesHelper.Success( 
                "Usuário cadastrado",
                new 
                {
                    usuario,
                }
            )
        );
    }
}