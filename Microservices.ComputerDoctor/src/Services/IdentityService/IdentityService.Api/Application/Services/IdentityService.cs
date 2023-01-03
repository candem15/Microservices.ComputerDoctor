
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IdentityService.Api.Application.Models;

namespace IdentityService.Api.Application.Services
{
    public class IdentityService : IIdentityService
    {
        public async Task<LoginResponseModel> Login(LoginRequestModel model)
        {

            var token = new Token();

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes("Owner of ComputerDoctor is Eray"));

            //Şifrelenmiş kimliği oluşturuyoruz.
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            //Oluşturulacak token ayarlarını veriyoruz.
            token.Expiration = DateTime.UtcNow.AddDays(5);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
                (
                audience: "ErayBerberoğlu",
                issuer: "ErayBerberoğlu",
                expires: token.Expiration,
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials,
                claims: new List<Claim>() { new(ClaimTypes.Name, model.Username) }
                );

            //Token oluşturucu sınıfından bir örnek alalım.
            JwtSecurityTokenHandler tokenHandler = new();
            token.AccessToken = tokenHandler.WriteToken(jwtSecurityToken);

            return new LoginResponseModel() { Username = model.Username, Token = token };
        }
    }
}