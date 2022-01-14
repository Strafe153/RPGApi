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
    [Migration("20220113205105_Password")]
    partial class Password
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

                    b.HasKey("Id");

                    b.ToTable("Players");
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
