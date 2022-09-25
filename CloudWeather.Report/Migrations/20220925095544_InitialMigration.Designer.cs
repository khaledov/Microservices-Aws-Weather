﻿// <auto-generated />
using System;
using CloudWeather.Report.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CloudWeather.Report.Migrations
{
    [DbContext(typeof(WeatherReportDbContext))]
    [Migration("20220925095544_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CloudWeather.Report.DataAccess.WeatherReport", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("AverageHigh")
                        .HasColumnType("numeric");

                    b.Property<decimal>("AverageLow")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("RainFallTotalInches")
                        .HasColumnType("numeric");

                    b.Property<decimal>("SnowTotalInches")
                        .HasColumnType("numeric");

                    b.Property<string>("ZipCode")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("weather_report", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
