using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UZI_Authentication.Middlewares;
using UZI_Authentication.Services;

namespace UZI_Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Certificate")]
    // [MiddlewareFilter(typeof(JWTAuthenticationMiddlewareAttribute))] // Optional double JWT from client middleware
    public class CertificateController : ControllerBase
    {
        private IConfiguration _config;

        public CertificateController(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IActionResult> Get()
        {
            X509Certificate2 cert = await HttpContext.Connection.GetClientCertificateAsync();
            var jwt = new JWTService(_config);
            var token = jwt.GenerateSecurityToken(
                (new DefaultCertificateParser()).Parse(HttpContext.Connection.ClientCertificate));
            return new JsonResult( new {jwt = token});
        }

    }
}