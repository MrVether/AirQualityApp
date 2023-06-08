﻿// <auto-generated />
using AirQualityApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AirQualityApp.Migrations
{
    [DbContext(typeof(AirQualityContext))]
    [Migration("20230605171245_init")]
    partial class init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("AirQualityApp.Models.Coordinates", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("Latitude")
                        .HasColumnType("REAL")
                        .HasColumnName("Latitude");

                    b.Property<double>("Longitude")
                        .HasColumnType("REAL")
                        .HasColumnName("Longitude");

                    b.HasKey("Id");

                    b.ToTable("Coordinates");
                });

            modelBuilder.Entity("AirQualityApp.Models.Country", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("TEXT")
                        .HasColumnName("Code");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Name");

                    b.HasKey("Code");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("AirQualityApp.Models.Date", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Local")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Local");

                    b.Property<string>("Utc")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Utc");

                    b.HasKey("Id");

                    b.ToTable("Date");
                });

            modelBuilder.Entity("AirQualityApp.Models.Measurement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("City")
                        .HasColumnType("TEXT")
                        .HasColumnName("City");

                    b.Property<int>("CoordinatesId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Country");

                    b.Property<int>("DateId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Location");

                    b.Property<string>("Parameter")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Parameter");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Unit");

                    b.Property<double>("Value")
                        .HasColumnType("REAL")
                        .HasColumnName("Value");

                    b.HasKey("Id");

                    b.HasIndex("CoordinatesId");

                    b.HasIndex("DateId");

                    b.ToTable("Measurement");
                });

            modelBuilder.Entity("AirQualityApp.Models.Measurement", b =>
                {
                    b.HasOne("AirQualityApp.Models.Coordinates", "Coordinates")
                        .WithMany()
                        .HasForeignKey("CoordinatesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AirQualityApp.Models.Date", "Date")
                        .WithMany()
                        .HasForeignKey("DateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Coordinates");

                    b.Navigation("Date");
                });
#pragma warning restore 612, 618
        }
    }
}