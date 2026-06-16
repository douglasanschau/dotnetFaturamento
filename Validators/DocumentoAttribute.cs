using System.ComponentModel.DataAnnotations;
using faturamentoClienteDotnet.Helpers;

namespace faturamentoClienteDotnet.Validators;

public class DocumentoAttribute : ValidationAttribute
{

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value == null){
            return new ValidationResult("Documento é obrigatório");
        }
        
        if (ValidatorHelper.GetTipoDocumento(value.ToString()!) ==  "INVÁLIDO"){
            return new ValidationResult("CPF ou CNPJ inválido");
        }

        return ValidationResult.Success;
    }
}