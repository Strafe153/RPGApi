﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RPGApi.Data;

#nullable disable

namespace RPGApi.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20220116040826_DefaultAdminUser")]
    partial class DefaultAdminUser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CharacterMount", b =>
                {
                    b.Property<Guid>("CharactersId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MountsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CharactersId", "MountsId");

                    b.HasIndex("MountsId");

                    b.ToTable("CharacterMount");
                });

            modelBuilder.Entity("CharacterSpell", b =>
                {
                    b.Property<Guid>("CharactersId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SpellsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CharactersId", "SpellsId");

                    b.HasIndex("SpellsId");

                    b.ToTable("CharacterSpell");
                });

            modelBuilder.Entity("CharacterWeapon", b =>
                {
                    b.Property<Guid>("CharactersId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("WeaponsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CharactersId", "WeaponsId");

                    b.HasIndex("WeaponsId");

                    b.ToTable("CharacterWeapon");
                });

            modelBuilder.Entity("RPGApi.Models.Character", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Health")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PlayerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Race")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("RPGApi.Models.Mount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Health")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Speed")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Mounts");
                });

            modelBuilder.Entity("RPGApi.Models.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Players");

                    b.HasData(
                        new
                        {
                            Id = new Guid("a288fd81-edf2-407a-ac6b-2b1d4fd1b74d"),
                            Name = "admin",
                            PasswordHash = new byte[] { 226, 132, 33, 154, 209, 4, 73, 58, 41, 18, 203, 244, 215, 52, 148, 91, 213, 248, 88, 192, 76, 107, 244, 179, 21, 207, 39, 223, 130, 247, 238, 140, 0, 99, 92, 185, 118, 43, 223, 50, 231, 68, 173, 202, 253, 225, 65, 150, 153, 65, 76, 243, 158, 233, 54, 4, 60, 114, 166, 7, 220, 51, 255, 125 },
                            PasswordSalt = new byte[] { 21, 76, 17, 9, 204, 37, 109, 213, 56, 71, 240, 23, 74, 7, 203, 0, 139, 117, 241, 252, 11, 58, 77, 37, 232, 31, 73, 233, 222, 87, 114, 34, 53, 57, 67, 69, 19, 207, 82, 210, 174, 134, 34, 68, 154, 148, 94, 129, 220, 149, 133, 186, 235, 251, 74, 98, 179, 148, 120, 208, 75, 138, 78, 186, 117, 120, 82, 52, 58, 237, 124, 171, 127, 48, 217, 14, 154, 97, 210, 146, 181, 128, 140, 197, 43, 185, 19, 178, 109, 144, 231, 108, 166, 250, 199, 209, 78, 255, 125, 50, 162, 209, 140, 245, 64, 248, 32, 240, 218, 84, 31, 101, 116, 30, 133, 239, 192, 136, 158, 46, 167, 88, 122, 96, 150, 157, 235, 242 },
                            Role = 0
                        });
                });

            modelBuilder.Entity("RPGApi.Models.Spell", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Damage")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Spells");
                });

            modelBuilder.Entity("RPGApi.Models.Weapon", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Damage")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Weapons");
                });

            modelBuilder.Entity("CharacterMount", b =>
                {
                    b.HasOne("RPGApi.Models.Character", null)
                        .WithMany()
                        .HasForeignKey("CharactersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RPGApi.Models.Mount", null)
                        .WithMany()
                        .HasForeignKey("MountsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CharacterSpell", b =>
                {
                    b.HasOne("RPGApi.Models.Character", null)
                        .WithMany()
                        .HasForeignKey("CharactersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RPGApi.Models.Spell", null)
                        .WithMany()
                        .HasForeignKey("SpellsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CharacterWeapon", b =>
                {
                    b.HasOne("RPGApi.Models.Character", null)
                        .WithMany()
                        .HasForeignKey("CharactersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RPGApi.Models.Weapon", null)
                        .WithMany()
                        .HasForeignKey("WeaponsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RPGApi.Models.Character", b =>
                {
                    b.HasOne("RPGApi.Models.Player", "Player")
                        .WithMany("Characters")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("RPGApi.Models.Player", b =>
                {
                    b.Navigation("Characters");
                });
#pragma warning restore 612, 618
        }
    }
}
