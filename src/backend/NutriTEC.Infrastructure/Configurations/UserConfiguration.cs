using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        // This mapping mirrors the existing app_user table and its uniqueness constraints.
        entity.ToTable("app_user");
        entity.HasKey(user => user.UserId).HasName("pk_app_user");

        entity.Property(user => user.UserId).HasColumnName("user_id");
        entity.Property(user => user.Birthday).HasColumnName("birthday").HasColumnType("date");
        entity.Property(user => user.Name).HasColumnName("name").HasMaxLength(80).IsUnicode(false);
        entity.Property(user => user.LastName).HasColumnName("last_name").HasMaxLength(80).IsUnicode(false);
        entity.Property(user => user.HashPassword).HasColumnName("hash_password").HasMaxLength(255).IsUnicode(false);
        entity.Property(user => user.Age).HasColumnName("age");
        entity.Property(user => user.Email).HasColumnName("email").HasMaxLength(255).IsUnicode(false);

        entity.HasIndex(user => user.Email).IsUnique().HasDatabaseName("uq_app_user_email");
    }
}
