using IdentityService.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using IdentityService.Application.Validators;

namespace IdentityService.Application;

public static class ApplicationServiceRegistration
{
    
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        //TODO add all services
        services.AddFluentValidationAutoValidation(); 
        services.AddValidatorsFromAssemblyContaining<UserRegisterDtoValidator>();
        services.AddScoped<IUserWorker, UserWorker>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordWorker, PasswordWorker>();
        return services;
    }
}