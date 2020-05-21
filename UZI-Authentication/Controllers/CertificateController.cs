using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace UZI_Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CertificateController : ControllerBase
    {

        // [Authorize]
        public JsonResult Get()
        {
            X509Certificate2 cert = new X509Certificate2(HttpContext.Connection.ClientCertificate);

            return new JsonResult((new DefaultCertificateParser()).Parse(HttpContext.Connection.ClientCertificate));
        }
    }
}