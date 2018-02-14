﻿// <auto-generated />
using dal;
using dal.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace dal.Migrations
{
    [DbContext(typeof(CryptoInvestContext))]
    [Migration("20180214113112_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("dal.models.Account", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Caption");

                    b.Property<int>("CurrencyID");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("CurrencyID");

                    b.HasIndex("UserID", "Name")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("dal.models.Currency", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Acronym")
                        .IsRequired();

                    b.Property<string>("CssImagePath");

                    b.Property<bool>("IsFiat");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("dal.models.Transaction", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<string>("Caption");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<decimal?>("Rate");

                    b.Property<int>("SourceAccountID");

                    b.Property<decimal>("SourceFees");

                    b.Property<int>("TargetAccountID");

                    b.Property<decimal>("TargetFees");

                    b.Property<byte>("Type");

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("SourceAccountID");

                    b.HasIndex("TargetAccountID");

                    b.HasIndex("UserID", "Date");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("dal.models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Login")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("dal.models.Account", b =>
                {
                    b.HasOne("dal.models.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("dal.models.Transaction", b =>
                {
                    b.HasOne("dal.models.Account", "SourceAccount")
                        .WithMany()
                        .HasForeignKey("SourceAccountID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("dal.models.Account", "TargetAccount")
                        .WithMany()
                        .HasForeignKey("TargetAccountID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
