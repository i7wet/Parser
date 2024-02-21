﻿// <auto-generated />
using System;
using DbContext.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace test.Migrations
{
    [DbContext(typeof(TestDbContext))]
    [Migration("20240214084447_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("test.Database.Models.ApartmentDb", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid?>("SubscriberDbId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("SubscriberDbId");

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("Apartments");
                });

            modelBuilder.Entity("test.Database.Models.SubscriberDb", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Subscribers");
                });

            modelBuilder.Entity("test.Database.Models.ApartmentDb", b =>
                {
                    b.HasOne("test.Database.Models.SubscriberDb", null)
                        .WithMany("Apartments")
                        .HasForeignKey("SubscriberDbId");
                });

            modelBuilder.Entity("test.Database.Models.SubscriberDb", b =>
                {
                    b.Navigation("Apartments");
                });
#pragma warning restore 612, 618
        }
    }
}