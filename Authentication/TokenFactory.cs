using System;
using System.Security.Cryptography;
using ReceitasApi.Authentication;

namespace Aplub.Api.Authentication {
    public class TokenFactory : ITokenFactory {
        public string GenerateRefreshToken (int size = 32) {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create ()) {
                rng.GetBytes (randomNumber);
                return Convert.ToBase64String (randomNumber);
            }
        }
    }
}