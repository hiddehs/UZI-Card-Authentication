using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders.Testing;
using UZI_Authentication.Middlewares;
using UZI_Authentication.Services;

namespace UZI_Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Certificate")]
    [MiddlewareFilter(typeof(JWTAuthenticationMiddlewareAttribute))]
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