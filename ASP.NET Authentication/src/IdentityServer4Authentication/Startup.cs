using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IdentityServer4Authentication.Data;
using IdentityServer4Authentication.Models;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Stores;
using IdentityServer4Authentication.Stores;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace IdentityServer4Authentication
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            
            // Add IdentityServer services
            services.AddSingleton<IClientStore, CustomClientStore>();

            services.AddIdentityServer()
                // .AddTemporarySigningCredential() // Can be used for testing until a real cert is available
                .AddSigningCredential(new X509Certificate2(Path.Combine("..", "..", "certs", "IdentityServer4Auth.pfx")))
                .AddInMemoryApiResources(MyApiResourceProvider.GetAllResources())
                .AddAspNetIdentity<ApplicationUser>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, RoleManager<IdentityRole> roleManager)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Seed database
            InitializeRoles(roleManager).Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseIdentity();

            // Using Identity implies cookie authentication.
            // Cookie authentication can also be added explicitly if not using Identity
            // app.UseCookieAuthentication();

            // Note that UseIdentityServer must come after UseIdentity in the pipeline
            app.UseIdentityServer();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            // External authentication middleware should come after app.UseIdentityServer (but before app.UseMvc) https://identityserver4.readthedocs.io/en/release/quickstarts/4_external_authentication.html
            
            app.UseStaticFiles();

            // Note that the UseMvc call must come after all authentication middleware (such as UseIdentity)
            app.UseMvcWithDefaultRoute();
        }

        // Initialize some test roles. In the real world, these would be setup explicitly by a role manager
        private string[] roles = new[] { "User", "Manager", "Administrator" };
        private async Task InitializeRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var newRole = new IdentityRole(role);
                    await roleManager.CreateAsync(newRole);
                    // In the real world, there might be claims associated with roles
                    // await roleManager.AddClaimAsync(newRole, new Claim("foo", "bar"))
                }
            }
        }
    }
}
