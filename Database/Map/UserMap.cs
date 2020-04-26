using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReceitasApi.Entities;

namespace ReceitasApi.Map {
    public class UserMap : IEntityTypeConfiguration<User> {
        public void Configure (EntityTypeBuilder<User> builder) {

            builder.ToTable ("Users");

            builder.Property (x => x.Name)
                .HasColumnType ("varchar(64)")
                .IsRequired ();
        }
    }
}