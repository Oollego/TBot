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
    internal class UserHistoryConfiguration : IEntityTypeConfiguration<UserHistory>
    {
        public void Configure(EntityTypeBuilder<UserHistory> builder)
        {
            builder.ToTable("user_history");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(64);
            builder.Property(x => x.Username).IsRequired().HasMaxLength(128);
            builder.Property(x => x.AppBundle).IsRequired().HasMaxLength(128);
            builder.Property(x => x.AppName).IsRequired().HasMaxLength(128);


        }
    }
}
