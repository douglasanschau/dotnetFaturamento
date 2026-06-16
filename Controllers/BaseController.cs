using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace faturamentoClienteDotnet.Controllers;

public abstract class BaseController : ControllerBase
{
    protected int UsuarioId =>
        int.Parse(
            User.FindFirst(
                ClaimTypes.NameIdentifier
            )!.Value
        );
}