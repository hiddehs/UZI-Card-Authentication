using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace UZI_Authentication.Services
{
    public class CertificateValidationService
    {
        public bool ValidateCertificate(X509Certificate2 clientCertificate)
        {
            // check rolcode
            Dictionary<string, string> certificateParser = (new DefaultCertificateParser()).Parse(clientCertificate);
            if(certificateParser != null && certificateParser["PassType"] == "Z")
                return true;
            return false;
        }
    }
}