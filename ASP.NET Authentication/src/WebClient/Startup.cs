using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace WebClient
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Add custom authorization handlers
            services.AddAuthorization(options =>
            {
                options.AddPolicy("OfficeNumberUnder400", policy => policy.Requirements.Add(new MaximumOfficeNumberRequirement(400)));
            });

            services.AddSingleton<IAuthorizationHandler, MaximumOfficeNumberAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Information on how the security token should be validated. In more common cases
            // (in which an Authority property is set for JwtBearerOptions, below), this may
            // not be necessary since default token validation parameters will be used to validate
            // the token with the authority.
            var certLocation = Path.Combine("..", "..", "certs", "IdentityServer4Auth.cer");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = "http://localhost:5000",
                IssuerSigningKey = new X509SecurityKey(new X509Certificate2(certLocation))
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                // Don't set authority since the issuing authority may not be available at run-time.
                // Instead, a valid issuer and signing key will be specified in token validation parameters
                // Authority = "http://localhost:5000/",
                
                Audience = "myAPIs",
                AutomaticAuthenticate = true,
                RequireHttpsMetadata = false,    // Set only for development scenarios - not in production
                TokenValidationParameters = tokenValidationParameters
            });

            // .UseMvc must come after authentication middleware
            app.UseMvc();
        }
    }
}
