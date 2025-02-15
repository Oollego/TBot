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
    internal class UserAppConfiguration : IEntityTypeConfiguration<UserApp>
    {
        public void Configure(EntityTypeBuilder<UserApp> builder)
        {
            builder.ToTable("user_app");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.AppName).IsRequired().HasMaxLength(128);
            builder.Property(x => x.AppBundle).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Secret).HasMaxLength(128);
            builder.Property(x => x.SecretKeyParam).HasMaxLength(256);

        }
    }
}
