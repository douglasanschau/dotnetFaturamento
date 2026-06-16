using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace faturamentoClienteDotnet.Helpers;

public class ResponsesHelper 
{
    public static ApiResponse<object> BadRequest(string message, ModelStateDictionary modelState)
    {
        return new ApiResponse<object> {
            Success = false, 
            StatusCode = 400,
            Message = message,
            Errors = GetErrors(modelState)
        };
    }

    public static  ApiResponse<object>  NotFound(string message)
    {
        return new ApiResponse<object> {
            Success = false,
            StatusCode = 404,
            Message = message
        };
    }

    public static  ApiResponse<object>  Unauthorized(string message)
    {
        return new ApiResponse<object> {
            Success = false,
            StatusCode = 404,
            Message = message
        };
    }

    public static  ApiResponse<object>  Conflict(string message)
    {
        return new ApiResponse<object> {
            Success = false,
            StatusCode = 409,
            Message = message
        };
    }

    public static  ApiResponse<object>  UnprocessableEntity(string message)
    {
        return new ApiResponse<object> {
            Success = false,
            StatusCode = 422,
            Message = message
        };
    }

    public static  ApiResponse<object>  Success(string message, object? data = null)
    {
        return new ApiResponse<object> {
            Success = true,
            StatusCode = 200,
            Message = message,
            Data = data
        };
    }

    public static List<string> GetErrors(ModelStateDictionary modelState)
    {
        return modelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();
    }
}