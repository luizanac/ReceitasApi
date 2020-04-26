using System;
using Microsoft.AspNetCore.Identity;

namespace ReceitasApi.Entities {
    public class User : IdentityUser<Guid> {
        public User () {

        }
        public string Name { get; set; }

        
        public bool Active { get; set; }
        public string UserType { get; set; }

        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }

        public void AddRefreshToken (string token, int daysToExpire) {
            RefreshToken = token;
            RefreshTokenExpiration = DateTime.Now.AddDays (daysToExpire);
        }

    }
}