using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace UZI_Authentication.Services
{
    public class CertificateValidationService
    {
        public bool ValidateCertificate(X509Certificate2 clientCertificate)
        {
            Console.WriteLine(clientCertificate.IssuerName);
            Console.WriteLine("Validation Service Triggered");
            // var cert = new X509Certificate2(Path.Combine("localhost_root_l1.pfx"), "1234");
            // if (clientCertificate.Thumbprint == cert.Thumbprint)
            // {
                // return true;
            // }
 
            return false;
        }
    }
}