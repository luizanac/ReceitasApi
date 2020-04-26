using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReceitasApi.Constants;
using ReceitasApi.Entities;

namespace ReceitasApi.Seed {
    public static class UserSeed {

        private static User CreateUser (string name, string email) {
            var hasher = new PasswordHasher<User> ();
            var user = new User () {
                Id = Guid.NewGuid (),
                Name = name,
                UserName = email,
                NormalizedUserName = email.ToUpper (),
                Email = email,
                NormalizedEmail = email.ToUpper (),
                UserType = UserTypes.Administrator,
                EmailConfirmed = true,
            };

            user.PasswordHash = hasher.HashPassword (user, "secret123");

            return user;
        }

        public static User[] Seed (EntityTypeBuilder<User> builder) {
            var user1 = CreateUser ("Luiz", "luizanacletozuchinali@gmail.com");
            var user2 = CreateUser ("Leonardo", "leonardo@gmail.com");
            var user3 = CreateUser ("Jair", "jair@gmail.com");
            var user4 = CreateUser ("Marlon", "marlon@gmail.com");
            var users = new User[] { user1, user2, user3, user4 };

            builder.HasData (users);

            return users;
        }
    }
}