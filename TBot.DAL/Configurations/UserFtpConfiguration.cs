using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBot.Domain.Entity;

namespace TBot.DAL.Configurations
{
    public class UserFtpConfiguration : IEntityTypeConfiguration<UserFtp>
    {
        public void Configure(EntityTypeBuilder<UserFtp> builder)
        {
            builder.ToTable("user_ftp_settings");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.SftpPort).IsRequired();
            builder.Property(x => x.SftpHost).IsRequired().HasMaxLength(256);
            builder.Property(x => x.SftpPassword).IsRequired().HasMaxLength(36);
            builder.Property(x => x.SftpLogin).IsRequired().HasMaxLength(128);

            builder.HasOne<UserData>(x => x.UserData)
                .WithOne(x => x.UserFtp)
                .HasForeignKey<UserFtp>(x => x.UserId);

        }
    }
}
