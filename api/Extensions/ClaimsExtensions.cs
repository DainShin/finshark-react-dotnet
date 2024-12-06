using System.Security.Claims;

namespace api.Extensions
{
    public static class ClaimsExtensions
    {
        // User.Claims는 클레임전체목록 가져옴
        // 특정 데이터(사용자 이름, 이메일) 자주 사용한다면 사용자 정의 메서드로 간소할 수 있음
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.Claims.SingleOrDefault(c => c.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")).Value;
        }
    }
}