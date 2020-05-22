using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UZI_Authentication.Services;

namespace UZI_Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SessionController
    {
        private IConfiguration _config;

        public SessionController(IConfiguration configuration)
        {
            _config = configuration;
        }

        public ActionResult Get()
        {
            JWTService jwt = new JWTService(_config);
            return new JsonResult(new
            {
                jwt = jwt.GenerateSecurityToken(new Dictionary<string, string>()
                {
                    {
                        "mode", "client"
                    }
                })
            });
        }
    }
}