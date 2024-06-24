using Login.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnName("Id");
            builder.Property(x => x.Username).IsRequired().HasColumnType("varchar(50)").HasColumnName("Username");
            builder.Property(x => x.Email).IsRequired().HasColumnType("varchar(100)").HasColumnName("Email");
            builder.Property(x => x.PasswordHash).IsRequired().HasColumnType("varchar(50)").HasColumnName("PasswordHash");
            builder.Property(x => x.Salt).IsRequired().HasColumnType("varchar(50)").HasColumnName("Salt");
            builder.Property(x => x.EmailConfirmed).IsRequired().HasColumnType("bit").HasColumnName("EmailConfirmed");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnType("datetime").HasColumnName("CreatedAt");
        }
    }
}
