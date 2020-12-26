using System.Security.Claims;
using Linx.Data;
using Linx.Services;
using Linx.Support;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Linx
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetValue<string>("ConnectionString");
            var authenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            services.AddHttpContextAccessor();

            services.Configure<Settings>(Configuration);

            services.Configure<CookiePolicyOptions>(options => {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
            });

            services.AddAuthentication(authenticationScheme)
                .AddCookie(authenticationScheme, options => {
                    options.LoginPath = "/users/login";
                    options.LogoutPath = "/users/logout";
                });

            services.AddScoped<IDateTimeService, DateTimeService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IRepository, SqlServerRepository>(sp => {
                var ctxAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<Settings>>();
                var userID = int.TryParse(ctxAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value, out var id) ? id : default(int?);
                return new SqlServerRepository(optionsMonitor, userID);
            });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // app.UseExceptionHandler("/Home/Error");
                // app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
