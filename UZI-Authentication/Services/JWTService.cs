using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace UZI_Authentication.Services
{
    public class JWTService  
    {  
        private readonly string _secret;  
        private readonly string _expDate;  
        private readonly string _issuer;  
  
        public JWTService(IConfiguration config)  
        {  
            _secret = config.GetSection("Jwt").GetSection("Secret").Value;  
            _issuer = config.GetSection("Jwt").GetSection("Issuer").Value;  
            _expDate = config.GetSection("Jwt").GetSection("ExpirationInMinutes").Value;  
        }  
  
        public string GenerateSecurityToken(Dictionary<string,string> authenticationPayload)  
        {  
            var tokenHandler = new JwtSecurityTokenHandler();  
            var key = Encoding.ASCII.GetBytes(_secret);  
            var tokenDescriptor = new SecurityTokenDescriptor  
            {  
                Subject = new ClaimsIdentity(new[]  
                {  
                    new Claim(ClaimTypes.Authentication, JsonConvert.SerializeObject(authenticationPayload))
                }),  
                Issuer = _issuer,
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_expDate)),  
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)  
            };  
  
            var token = tokenHandler.CreateToken(tokenDescriptor);  
  
            return tokenHandler.WriteToken(token);  
  
        }  
    }  
}