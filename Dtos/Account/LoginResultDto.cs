using System;

namespace ReceitasApi.Dtos.Account {
    public class LoginResultDto {
        public LoginResultDto (string type, string accessToken, string refreshToken, DateTime expiration, string userType, string userName) {
            Type = type;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Expiration = expiration;
            UserType = userType;
            UserName = userName;
        }
        public string UserName { get; set; }
        public string Type { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string UserType { get; set; }
        public DateTime Expiration { get; set; }
    }
}