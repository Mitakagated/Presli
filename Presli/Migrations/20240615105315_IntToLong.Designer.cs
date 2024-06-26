﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Presli.Models;

#nullable disable

namespace Presli.Migrations
{
    [DbContext(typeof(CurrencyInfoContext))]
    [Migration("20240615105315_IntToLong")]
    partial class IntToLong
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Presli.Models.CurrencyInfo", b =>
                {
                    b.Property<decimal>("DiscordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<long>("BettingCurrency")
                        .HasColumnType("bigint");

                    b.Property<long>("Mito")
                        .HasColumnType("bigint");

                    b.HasKey("DiscordId");

                    b.ToTable("CurrencyInfos");
                });
#pragma warning restore 612, 618
        }
    }
}
