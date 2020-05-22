using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace UZI_Authentication
{
    public interface ICertificateParser
    {
        Dictionary<string, string> Parse(X509Certificate2 certificate);
    }
}