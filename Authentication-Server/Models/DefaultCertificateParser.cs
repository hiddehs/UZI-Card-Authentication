using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace UZI_Authentication
{
    public class DefaultCertificateParser : ICertificateParser
    {
        public Dictionary<string, string> Parse(X509Certificate2 certificate)
        {
            Dictionary<string, string> certData = new Dictionary<string, string>();
            if (certificate == null)
                return certData;
            DefaultCertificateParser.CertificateParser
                certificateParser = new DefaultCertificateParser.CertificateParser(certificate);
            certData.Add("PassholderDescription", certificateParser.PassholderDescription);
            certData.Add("PassholderName", certificateParser.PassholderName);
            certData.Add("SubjectName", certificateParser.SubjectName);
            certData.Add("CommonName", certificateParser.CommonName);
            certData.Add("Title", certificateParser.Title);
            certData.Add("OrganizationName", certificateParser.OrganizationName);
            certData.Add("UziNumber", certificateParser.UziNumber);
            certData.Add("PassType", certificateParser.PassType.ToString());

            bool flag = certificateParser.IsValidUserPass;
            string str1 = flag.ToString();
            certData.Add("IsValidUserPass", str1);

            flag = certificateParser.IsServerPass;
            string str2 = flag.ToString();
            certData.Add("IsServerPass", str2);

            flag = certificateParser.IsAgbCodeSpecified;
            string str3 = flag.ToString();
            
            certData.Add("IsAgbCodeSpecified", str3);
            certData.Add("AgbCode", certificateParser.AgbCode);
            certData.Add("UziNumberOid", certificateParser.UziNumberOid);
            certData.Add("RoleCode", certificateParser.RoleCode);
            certData.Add("Role", certificateParser.Role);
            certData.Add("UziRegisterSubscriberNumber", certificateParser.UziRegisterSubscriberNumber);
            certData.Add("UziRegisterSubscriberOid", certificateParser.UziRegisterSubscriberOid);
            return certData;
        }

        private sealed class CertificateParser
        {
            private static string UnknownAgbCode = "00000000";

            private static readonly string[,] RoleCodes = new string[67, 2]
            {
                {
                    "00.000",
                    "Medewerker"
                },
                {
                    "01.000",
                    "Arts"
                },
                {
                    "01.002",
                    "Internist-Allergoloog"
                },
                {
                    "01.003",
                    "Anesthesioloog"
                },
                {
                    "01.004",
                    "Apotheekhoudende huisarts"
                },
                {
                    "01.008",
                    "Arts arbeid gezond./bedrijfarts"
                },
                {
                    "01.010",
                    "Cardioloog"
                },
                {
                    "01.011",
                    "Cardiothoracaal chirurg"
                },
                {
                    "01.012",
                    "Dermatoloog"
                },
                {
                    "01.013",
                    "Gastro-enteroloog"
                },
                {
                    "01.014",
                    "Chirurg"
                },
                {
                    "01.015",
                    "Huisarts"
                },
                {
                    "01.016",
                    "Internist"
                },
                {
                    "01.018",
                    "Keel- neus- en oorarts"
                },
                {
                    "01.019",
                    "Kinderarts"
                },
                {
                    "01.020",
                    "Arts klinische chemie"
                },
                {
                    "01.021",
                    "Klinisch geneticus"
                },
                {
                    "01.022",
                    "Klinisch geriater"
                },
                {
                    "01.023",
                    "Longarts"
                },
                {
                    "01.024",
                    "Arts-microbioloog"
                },
                {
                    "01.025",
                    "Neurochirurg"
                },
                {
                    "01.026",
                    "Neuroloog"
                },
                {
                    "01.030",
                    "Nucleair geneeskundige"
                },
                {
                    "01.031",
                    "Oogarts"
                },
                {
                    "01.032",
                    "Orthopedisch chirurg"
                },
                {
                    "01.033",
                    "Patholoog"
                },
                {
                    "01.034",
                    "Plastisch chirurg"
                },
                {
                    "01.035",
                    "Psychiater"
                },
                {
                    "01.039",
                    "Radioloog"
                },
                {
                    "01.040",
                    "Radiotherapeut"
                },
                {
                    "01.041",
                    "Reumatoloog"
                },
                {
                    "01.042",
                    "RevalidatieArts"
                },
                {
                    "01.055",
                    "Arts maatschappij en gezondheid"
                },
                {
                    "01.045",
                    "Uroloog"
                },
                {
                    "01.046",
                    "Gynaecoloog"
                },
                {
                    "01.047",
                    "Verpleeghuisarts"
                },
                {
                    "01.048",
                    "Arts arbeid gezond./verzekeringsarts"
                },
                {
                    "01.050",
                    "Zenuwarts"
                },
                {
                    "01.055",
                    "Arts maatschappij en gezondheid"
                },
                {
                    "01.056",
                    "Arts verstandelijk gehandicapten"
                },
                {
                    "02.000",
                    "Tandarts"
                },
                {
                    "02.053",
                    "Orthodontist"
                },
                {
                    "02.054",
                    "Kaakchirurg"
                },
                {
                    "03.000",
                    "Verloskundige"
                },
                {
                    "04.000",
                    "Fysiotherapeut"
                },
                {
                    "16.000",
                    "Psychotherapeut"
                },
                {
                    "17.000",
                    "Apotheker"
                },
                {
                    "17.060",
                    "Ziekenhuisapotheker"
                },
                {
                    "17.075",
                    "Openbare apotheker"
                },
                {
                    "25.000",
                    "GZ-psycholoog"
                },
                {
                    "25.061",
                    "Klinisch psycholoog"
                },
                {
                    "30.000",
                    "Verpleegkundige"
                },
                {
                    "83.000",
                    "Apothekersassistent"
                },
                {
                    "85.000",
                    "Tandprotheticus"
                },
                {
                    "86.000",
                    "Verzorgende in de individuele gezondheidszorg"
                },
                {
                    "87.000",
                    "Optometrist"
                },
                {
                    "88.000",
                    "Huidtherapeut"
                },
                {
                    "89.000",
                    "Dietist"
                },
                {
                    "90.000",
                    "Ergotherapeut"
                },
                {
                    "91.000",
                    "Logopedist"
                },
                {
                    "92.000",
                    "Mondhygienist"
                },
                {
                    "93.000",
                    "Oefentherapeut Mensendieck"
                },
                {
                    "94.000",
                    "Oefentherapeut Cesar"
                },
                {
                    "95.000",
                    "Orthoptist"
                },
                {
                    "96.000",
                    "Podotherapeut"
                },
                {
                    "97.000",
                    "Radiodiagnostisch laborant"
                },
                {
                    "98.000",
                    "Radiotherapeutisch laborant"
                }
            };

            private DefaultCertificateParser.CertificateParser.SubjectOtherNameHolder _subjectOtherName;
            private readonly X509Certificate2 _x509Certificate;

            public CertificateParser(X509Certificate2 x509Certificate)
            {
                this._x509Certificate = x509Certificate;
            }

            public string PassholderDescription
            {
                get
                {
                    return "UZI-nummer: " + this.UziNumber + Environment.NewLine + "Naam      : " +
                           this.PassholderName + Environment.NewLine + "Functie   : " + this.Role + Environment.NewLine;
                }
            }

            public string PassholderName
            {
                get { return this._x509Certificate.GetNameInfo(X509NameType.SimpleName, false); }
            }

            public string SubjectName
            {
                get { return this._x509Certificate.Subject; }
            }

            public string CommonName
            {
                get
                {
                    string upper = this.SubjectName.ToUpper();
                    int num1 = 3;
                    int startIndex = upper.IndexOf("CN=", StringComparison.InvariantCulture);
                    if (startIndex == -1)
                    {
                        startIndex = upper.IndexOf("CN =", StringComparison.InvariantCulture);
                        num1 = 4;
                        if (startIndex == -1)
                            return string.Empty;
                    }

                    int num2 = upper.IndexOf(",", startIndex, StringComparison.InvariantCulture);
                    if (num2 == -1)
                        return this.SubjectName.Substring(startIndex + num1).Trim();
                    return this.SubjectName.Substring(startIndex + num1, num2 - (startIndex + num1)).Trim();
                }
            }

            public string Title
            {
                get
                {
                    string upper = this.SubjectName.ToUpper();
                    int num1 = 2;
                    int startIndex = upper.IndexOf("T=", StringComparison.InvariantCulture);
                    if (startIndex == -1)
                    {
                        startIndex = upper.IndexOf("T =", StringComparison.InvariantCulture);
                        num1 = 3;
                        if (startIndex == -1)
                            return string.Empty;
                    }

                    int num2 = upper.IndexOf(",", startIndex, StringComparison.InvariantCulture);
                    if (num2 == -1)
                        return this.SubjectName.Substring(startIndex + num1).Trim();
                    return this.SubjectName.Substring(startIndex + num1, num2 - (startIndex + num1)).Trim();
                }
            }

            public string OrganizationName
            {
                get
                {
                    string upper = this.SubjectName.ToUpper();
                    int num1 = 2;
                    int startIndex = upper.IndexOf("O=", StringComparison.InvariantCulture);
                    if (startIndex == -1)
                    {
                        startIndex = upper.IndexOf("O =", StringComparison.InvariantCulture);
                        num1 = 3;
                        if (startIndex == -1)
                            return string.Empty;
                    }

                    int num2 = upper.IndexOf(",", startIndex, StringComparison.InvariantCulture);
                    if (num2 == -1)
                        return this.SubjectName.Substring(startIndex + num1).Trim();
                    return this.SubjectName.Substring(startIndex + num1, num2 - (startIndex + num1)).Trim();
                }
            }

            public string UziNumber
            {
                get { return this.SubjectOtherName.UziNumber; }
            }

            public char PassType
            {
                get
                {
                    switch (this.SubjectOtherName.PassType)
                    {
                        case 'M':
                        case 'N':
                        case 'S':
                        case 'Z':
                            return this.SubjectOtherName.PassType;
                        default:
                            throw new ArgumentException(
                                "Invalid passtype: " + this.SubjectOtherName.PassType.ToString());
                    }
                }
            }

            public bool IsValidUserPass
            {
                get
                {
                    try
                    {
                        switch (this.PassType)
                        {
                            case 'M':
                            case 'N':
                            case 'Z':
                                return true;
                            default:
                                return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }

            public bool IsServerPass
            {
                get
                {
                    try
                    {
                        return this.PassType == 'S';
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }

            public bool IsAgbCodeSpecified
            {
                get
                {
                    if (!string.IsNullOrEmpty(this.AgbCode))
                        return !this.AgbCode.Equals(DefaultCertificateParser.CertificateParser.UnknownAgbCode);
                    return false;
                }
            }

            public string AgbCode
            {
                get
                {
                    try
                    {
                        return this.SubjectOtherName.AgbCode;
                    }
                    catch (Exception ex)
                    {
                        return DefaultCertificateParser.CertificateParser.UnknownAgbCode;
                    }
                }
            }

            public string UziNumberOid
            {
                get
                {
                    return this.SubjectOtherName.PassType.Equals('S') ? "2.16.528.1.1007.3.2" : "2.16.528.1.1007.3.1";
                }
            }

            public string RoleCode
            {
                get { return this.SubjectOtherName.RoleCode; }
            }

            public string Role
            {
                get { return DefaultCertificateParser.CertificateParser.GetRole(this.SubjectOtherName.RoleCode); }
            }

            public string UziRegisterSubscriberNumber
            {
                get { return this.SubjectOtherName.UziRegisterSubscriberNumber; }
            }

            public string UziRegisterSubscriberOid
            {
                get { return this.SubjectOtherName.UziRegisterSubcriberOid; }
            }

            private static string GetRole(string code)
            {
                for (int index = 0; index < DefaultCertificateParser.CertificateParser.RoleCodes.Length / 2; ++index)
                {
                    if (code.Equals(DefaultCertificateParser.CertificateParser.RoleCodes[index, 0]))
                        return DefaultCertificateParser.CertificateParser.RoleCodes[index, 1];
                }

                return "Onbekend";
            }

            private DefaultCertificateParser.CertificateParser.SubjectOtherNameHolder SubjectOtherName
            {
                get
                {
                    if (this._subjectOtherName == null)
                        this._subjectOtherName =
                            new DefaultCertificateParser.CertificateParser.SubjectOtherNameHolder(
                                this._x509Certificate);
                    return this._subjectOtherName;
                }
            }

            private class SubjectOtherNameHolder
            {
                private readonly string[] _fields;

                public SubjectOtherNameHolder(X509Certificate2 cert)
                {
                    foreach (X509Extension extension in cert.Extensions)
                    {
                        if (extension.Oid.Value.Equals("2.5.29.17"))
                        {
                            string str1 = new AsnEncodedData(extension.Oid, extension.RawData).Format(false);
                            var data = Encoding.UTF8.GetString(extension.RawData).ToString();

                            if (data.Contains("="))
                            {
                                this._fields = data.Split('=')[1].Split('-');
                            }
                            else if (data.Contains("@"))
                            {
                                this._fields = data.Split('@')[2].Split('-');
                            }


                            break;
                        }
                    }
                }

                public string UziNumber
                {
                    get { return this._fields[2]; }
                }

                public char PassType
                {
                    get { return char.ToUpper(this._fields[3][0]); }
                }

                public string UziRegisterSubcriberOid
                {
                    get { return "2.16.528.1.1007.3.3"; }
                }

                public string UziRegisterSubscriberNumber
                {
                    get { return this._fields[4]; }
                }

                public string RoleCode
                {
                    get { return this._fields[5]; }
                }

                public string AgbCode
                {
                    get { return this._fields[6]; }
                }

                private static string ByteStringToString(string str)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int startIndex = 0; startIndex < str.Length - 1; startIndex += 3)
                    {
                        stringBuilder.Append(Convert.ToChar(byte.Parse(str.Substring(startIndex, 2),
                            NumberStyles.HexNumber)));
                    }

                    return stringBuilder.ToString();
                }
            }
        }
    }
}