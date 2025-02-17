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
    internal class UserAppSettingConfiguration : IEntityTypeConfiguration<UserAppSetting>
    {
        public void Configure(EntityTypeBuilder<UserAppSetting> builder)
        {
            builder.ToTable("user_app_settings");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.AppName).IsRequired().HasMaxLength(128);
            builder.Property(x => x.AppBundle).IsRequired().HasMaxLength(128);

            builder.HasOne<UserData>(x => x.UserData)
                .WithOne(x => x.UserAppSetting)
                .HasForeignKey<UserAppSetting>(x => x.UserId);

        }
    }
}
