using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using UZI_Authentication.Services;

namespace UZI_Authentication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddSingleton<CertificateValidationService>();
            
            services.AddAuthentication(
                options =>
                {
                    options.DefaultScheme = CertificateAuthenticationDefaults.AuthenticationScheme;
                }) 
                .AddJwtBearer(options =>    
                {    
                    options.TokenValidationParameters = new TokenValidationParameters    
                    {    
                        ValidateIssuer = true,    
                        ValidateAudience = true,    
                        ValidateLifetime = true,    
                        ValidateIssuerSigningKey = true,    
                        ValidIssuer = Configuration["Jwt:Issuer"],    
                        ValidAudience = Configuration["Jwt:Issuer"],    
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))    
                    };    
                }).AddCertificate(options =>
                {
                    options.AllowedCertificateTypes = CertificateTypes.Chained;
                    options.RevocationMode = X509RevocationMode.NoCheck;
                    options.RevocationFlag = X509RevocationFlag.EntireChain;
                    
                    options.Events = new CertificateAuthenticationEvents
                    {
                        OnCertificateValidated = context =>
                        {
                            var validationService = context.HttpContext.RequestServices.GetService<CertificateValidationService>();
            
                            if (validationService.ValidateCertificate(context.ClientCertificate))
                            {
                                context.Success();
                            }
                            else
                            {
                                context.Fail("Invalid Custom Validation Certificate");
                            }
            
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            Log.Error("Original Certificate validation failed.");
                            context.Fail("Invalid Certificate");
                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}