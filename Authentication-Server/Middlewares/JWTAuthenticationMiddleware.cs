using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace UZI_Authentication.Middlewares
{
    public class JWTAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        private string _secret;

        public JWTAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _secret = configuration.GetSection("Jwt").GetSection("Secret").Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string AuthenticationHeaderString = context.Request.Headers["Authentication"];
            if (!string.IsNullOrWhiteSpace(AuthenticationHeaderString))
            {
                string token = AuthenticationHeaderString.ToString().Split("Bearer ")[1];

                var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secret));

                var myIssuer = "UZI-Card-Authentication-Node";

                var tokenHandler = new JwtSecurityTokenHandler();
                try
                {
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidIssuer = myIssuer,
                        IssuerSigningKey = mySecurityKey
                    }, out SecurityToken validatedToken);
                    await _next(context);
                    return;
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToString());
                }
            }

            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
        }
    }

    public class JWTAuthenticationMiddlewareAttribute
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<JWTAuthenticationMiddleware>();
        }
    }
}