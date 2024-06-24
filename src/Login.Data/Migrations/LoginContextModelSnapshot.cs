﻿// <auto-generated />
using System;
using Login.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Login.Data.Data.Migrations
{
    [DbContext(typeof(LoginContext))]
    partial class LoginContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Login.Core.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasColumnName("Email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit")
                        .HasColumnName("EmailConfirmed");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("PasswordHash");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("Salt");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("Username");

                    b.HasKey("Id");

                    b.ToTable("Users", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
