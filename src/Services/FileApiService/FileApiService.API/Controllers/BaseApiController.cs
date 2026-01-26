using FileApiService.Application;
using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Application.Exceptions.FluentResultsErrors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{

    public BaseApiController()
    {
    }
    protected Guid CurrentUserId
    {
        get
        {
            if (Request.Headers.TryGetValue("Id", out var headerValue) && 
                Guid.TryParse(headerValue, out var id))
            {
                return id;
            }
            throw new UnauthorizedAccessException("User ID is missing or invalid");
        }
    }
    
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            if (typeof(T) == typeof(string) && result.Value is string s && string.IsNullOrEmpty(s))
                return Ok();
                 
            return Ok(result.Value);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    protected ActionResult HandleResult(Result result)
    {
        if (result.IsSuccess) return Ok();

        var error = result.Errors.FirstOrDefault();
        if (error is null) return StatusCode(500);

        return error switch
        {
            FileNotFoundError => NotFound(new { error.Message }),
            DirectoryNotFoundError => NotFound(new { error.Message }),
            
            UnauthorizedAccessError => Unauthorized(new { error.Message }),
            
            FileAlreadyExistsError => Conflict(new { error.Message }),
            
            InvalidTypeOfItemError => BadRequest(new { error.Message }),
            
            InvalidParentError => BadRequest(new { error.Message }),
            
            _ => BadRequest(new { error.Message })
        };
    }
}