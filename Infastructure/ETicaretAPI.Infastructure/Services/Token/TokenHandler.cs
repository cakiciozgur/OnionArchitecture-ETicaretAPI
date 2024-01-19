using ETicaretAPI.Application.Abstractions.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infastructure.Services.Token
{
    public class TokenHandler : ITokenHandler
    {
        readonly IConfiguration _configuration;

        public TokenHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Application.DTOs.Token CreateAccessToken(int second)
        {
            Application.DTOs.Token token = new();


            //security key simetriği alınır
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));


            //şifrelenmiş kimlik oluşturulue
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);


            token.Expiration = DateTime.UtcNow.AddSeconds(second);


            //oluşturulacak token detayları
            JwtSecurityToken jwtSecurityToken = new(
                audience: _configuration["Token:Audience"],
                issuer: _configuration["Token:Issuer"],
                expires: token.Expiration,
                notBefore: DateTime.UtcNow, // hangi andan itibaren devreye gireceği
                signingCredentials : signingCredentials
                );


            // token oluşturma
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            token.AccessToken = tokenHandler.WriteToken(jwtSecurityToken);

            token.RefreshToken = CreateRefreshToken();

            return token;
        }

        public string CreateRefreshToken()
        {
            byte[] number = new byte[32];

            using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create(); // using eklenirse scopelar arasında aktif kalır sonra dispose edilir

            randomNumberGenerator.GetBytes(number);

            return Convert.ToBase64String(number);
        }
    }
}
