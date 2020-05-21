using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UZI_Authentication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(o =>
                    {
                        o.ConfigureHttpsDefaults(o =>
                        {
                            o.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                            // o.AllowAnyClientCertificate();
                            
                            o.ClientCertificateValidation = (certificate2, chain, arg3) =>
                            {
                                Console.WriteLine( JsonSerializer.Serialize((new DefaultCertificateParser()).Parse(certificate2)).ToString());
                                return true;
                            };

                        });
                    });
                });
        }
    }
}