﻿// <auto-generated />
using System;
using ECard.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ECard.Data.Migrations
{
    [DbContext(typeof(ECardDataContext))]
    [Migration("20190705140720_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ECard.Entities.Entities.ECardDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CreatedByName");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description");

                    b.Property<string>("DomainName");

                    b.Property<int>("Id_User");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("ModifiedByName");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("Id_User");

                    b.ToTable("ECardDetail");
                });

            modelBuilder.Entity("ECard.Entities.Entities.Rsvp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AttCount");

                    b.Property<string>("Attendance");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CreatedByName");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Email");

                    b.Property<int>("Id_EcardDetail");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("ModifiedByName");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Name");

                    b.Property<string>("Wishes");

                    b.HasKey("Id");

                    b.HasIndex("Id_EcardDetail");

                    b.ToTable("Rsvp");
                });

            modelBuilder.Entity("ECard.Entities.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CreatedByName");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("ModifiedByName");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<byte[]>("PasswordHash");

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ECard.Entities.Entities.ECardDetail", b =>
                {
                    b.HasOne("ECard.Entities.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("Id_User")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ECard.Entities.Entities.Rsvp", b =>
                {
                    b.HasOne("ECard.Entities.Entities.ECardDetail", "ECardDetail")
                        .WithMany()
                        .HasForeignKey("Id_EcardDetail")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
