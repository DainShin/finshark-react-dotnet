using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Interfaces;
using api.Models;
using Microsoft.IdentityModel.Tokens;

namespace api.Service
{
    public class TokenService : ITokenService
    {
        // IConfiguration: appsettings.json
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config) 
        {
            _config = config;
            // SigningKey -> 바이트배열로 변환 -> SymmetricSecurityKey 객제 생성 -> 서명 생성
            // 서명구조: Base64(Header) + Base64(Payload) + 서명 
            // SymmetricSecurityKey: JWT보호, 서명검증(클라이언트가 보낸 토큰이 서버에서 발급한것인지를 확인), 간편성
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"])); 
        }
        public string CreateToken(AppUser user)
        {
            // Claim 생성: JWT payload 부분에 포함될 사용자 정보 => 사용자를 식별할 수 있는 데이터를 토큰에 포함
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
            };

            // form of encryption
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            
            // create the token object
            // SecurityTokenDescriptor: 이 객체를 사용하여 토큰에 포함될 정보와 속성을 설정
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // 토큰에 포함될 사용자정보 설정
                Expires = DateTime.Now.AddDays(7), // 토큰 만료 시간
                SigningCredentials = creds, // 토큰을 보호하기 위한 서명 자각증명
                Issuer = _config["JWT:Issuer"], // 발급자 정보
                Audience = _config["JWT:Audience"] // 대상자 정보
            };

            // create tokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            // create token, utilizing the tokenHandler and tokenDescriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // return the token in the form of string
            return tokenHandler.WriteToken(token);
        }
    }
}