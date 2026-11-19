using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Grid.CustomLocalization;
using Microsoft.Extensions.Options;

namespace Grid.Pages
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Trial License Key
        private const string SyncfusionLicenseKey = "NAsnBzUROFJ8W252S0diV1NEdFlfXGFUf1pmRWRTf1h6dFxNYlxTQndeQ1RjTn5Vd0FiXnZXe3dQ";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddRazorPages();
            services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");
            services.AddMvc().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
                o.JsonSerializerOptions.DictionaryKeyPolicy = null;
            });
            // Configure Localization Options
            services.AddSingleton<MyCustomRequestCultureProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<RequestLocalizationOptions> locOptions, MyCustomRequestCultureProvider myCustomRequestCultureProvider)
        {
            if (string.IsNullOrWhiteSpace(SyncfusionLicenseKey))
            {
                throw new InvalidOperationException(
                    "Set the SyncfusionLicenseKey constant in Startup.cs to your Syncfusion license key.");
            }

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SyncfusionLicenseKey);

            app.UseForwardedHeaders();

            var basePath = Configuration.GetValue<string>("BasePath") ?? "/datagrid-crud";
            if (!string.IsNullOrEmpty(basePath))
            {
                if (!basePath.StartsWith("/"))
                {
                    basePath = "/" + basePath;
                }
                basePath = basePath.TrimEnd('/');

                app.UsePathBase(basePath);

                // Ensure incoming requests matching the root redirect to the base path
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path == "/" || string.IsNullOrEmpty(context.Request.Path.Value))
                    {
                        context.Response.Redirect(basePath + "/");
                        return;
                    }
                    await next();
                });
            }

            // Middleware for Content-Security-Policy and ScriptNonce
            app.Use(async (context, next) =>
            {
                byte[] nonceBytes = new byte[32];
                RandomNumberGenerator.Fill(nonceBytes);

                string nonceValue = Convert.ToBase64String(nonceBytes);
                context.Items.Add("ScriptNonce", nonceValue);
                context.Response.Headers.Add("Content-Security-Policy", string.Format(
                "script-src 'self' 'nonce-{0}' cdn.syncfusion.com;" +
                "style-src-elem 'self' cdn.syncfusion.com fonts.googleapis.com;" +
                "font-src 'self' data: fonts.gstatic.com;" +
                "object-src 'none';", nonceValue));
                await next();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
                        
            locOptions.Value.RequestCultureProviders.Insert(0, myCustomRequestCultureProvider);
            app.UseRequestLocalization(locOptions.Value);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
