using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace HistoricViewer.Security.Configs;

public static class AppSecurityConfigurations
{


    public static IServiceCollection AddSecurityModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Azure AD JWT Bearer authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));
        
        services.PostConfigure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                var azureAd = configuration
                    .GetSection("AzureAd")
                    .Get<MicrosoftIdentityOptions>();

                if (azureAd is null || string.IsNullOrWhiteSpace(azureAd.ClientId))
                {
                    throw new InvalidOperationException("AzureAd:ClientId is missing.");
                }
                
                var clientId = azureAd!.ClientId;

                options.TokenValidationParameters.ValidAudiences = new[]
                {
                    clientId,
                    $"api://{clientId}"
                };
            });
        
        
        // fallback policy → require auth for everything by default
        // specific endpoints → mark as anonymous
        // controller/action level → use [AllowAnonymous] or [Authorize]
        
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
            options.AddPolicy("ApiReader", p => p.RequireRole("ApiReader"));
        });
        return services;
    }

}