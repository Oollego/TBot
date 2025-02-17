using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBot.Domain.Entity;

namespace TBot.DAL.Configurations
{
    internal class UserDataConfiguration : IEntityTypeConfiguration<UserData>
    {
        public void Configure(EntityTypeBuilder<UserData> builder)
        {
            builder.ToTable("users");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserName).IsRequired().HasMaxLength(128);
            builder.Property(x => x.PasswordSalt).IsRequired().HasMaxLength(128);
            builder.Property(x => x.PasswordDk).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Role).HasMaxLength(64);

            builder.HasMany(x => x.UserApps)
                .WithOne(x => x.UserData)
                .HasForeignKey(x => x.UserId)
                .HasPrincipalKey(x => x.Id);
        }
    }
}
