﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WeatherApp.Data;

#nullable disable

namespace WeatherApp.Migrations
{
    [DbContext(typeof(WeatherDbContext))]
    [Migration("20250228105346_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WeatherApp.Entity.WeatherRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("Cloudiness")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<double?>("DewPoint")
                        .HasColumnType("double precision");

                    b.Property<int?>("HorizontalVisibility")
                        .HasColumnType("integer");

                    b.Property<double?>("Humidity")
                        .HasColumnType("double precision");

                    b.Property<int?>("LowerCloudLimit")
                        .HasColumnType("integer");

                    b.Property<double?>("Pressure")
                        .HasColumnType("double precision");

                    b.Property<double?>("Temperature")
                        .HasColumnType("double precision");

                    b.Property<TimeOnly?>("Time")
                        .HasColumnType("time without time zone");

                    b.Property<string>("WeatherPhenomena")
                        .HasColumnType("text");

                    b.Property<string>("WindDirection")
                        .HasColumnType("text");

                    b.Property<double?>("WindSpeed")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("WeatherRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
