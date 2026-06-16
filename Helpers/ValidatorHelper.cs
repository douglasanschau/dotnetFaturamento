using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace faturamentoClienteDotnet.Helpers;

public class ValidatorHelper 
{
    public static string GetTipoDocumento(string documento)
    {
        documento = new string(documento.Where(char.IsDigit).ToArray());

        switch(documento.Length){
            case 11:
                return "CPF";
            case 14:
                return "CNPJ";
            default:
                return "INVÁLIDO";
          
        }
    }
}