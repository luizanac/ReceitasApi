using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReceitasApi.Entities;
using ReceitasApi.Map;
using ReceitasApi.Seed;

namespace ReceitasApi.Database {
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid> {
        public ApplicationDbContext (DbContextOptions options) : base (options) { }
        protected override void OnModelCreating (ModelBuilder builder) {
            builder.ApplyConfiguration (new UserMap ());

            var users = UserSeed.Seed (builder.Entity<User> ());

            base.OnModelCreating (builder);
        }
        public override DbSet<User> Users { get; set; }

    }
}