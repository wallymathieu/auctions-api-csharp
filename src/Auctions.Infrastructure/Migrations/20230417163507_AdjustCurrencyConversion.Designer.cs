﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Wallymathieu.Auctions.Infrastructure.Data;

#nullable disable

namespace App.Migrations
{
    [DbContext(typeof(AuctionDbContext))]
    [Migration("20230417163507_AdjustCurrencyConversion")]
    partial class AdjustCurrencyConversion
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Auctions.Domain.BidEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTimeOffset>("At")
                        .HasColumnType("datetimeoffset");

                    b.Property<long?>("AuctionId")
                        .HasColumnType("bigint");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.HasKey("Id");

                    b.HasIndex("AuctionId");

                    b.ToTable("Bids", (string)null);
                });

            modelBuilder.Entity("Auctions.Domain.TimedAscendingAuction", b =>
                {
                    b.Property<long>("AuctionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("AuctionId"));

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("EndsAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("Expiry")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("StartsAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.HasKey("AuctionId");

                    b.ToTable("Auctions", (string)null);
                });

            modelBuilder.Entity("Auctions.Domain.BidEntity", b =>
                {
                    b.HasOne("Auctions.Domain.TimedAscendingAuction", null)
                        .WithMany("Bids")
                        .HasForeignKey("AuctionId");

                    b.OwnsOne("Auctions.Domain.Amount", "Amount", b1 =>
                        {
                            b1.Property<long>("BidEntityId")
                                .HasColumnType("bigint");

                            b1.Property<int>("Currency")
                                .HasColumnType("int");

                            b1.Property<long>("Value")
                                .HasColumnType("bigint");

                            b1.HasKey("BidEntityId");

                            b1.ToTable("Bids");

                            b1.WithOwner()
                                .HasForeignKey("BidEntityId");
                        });

                    b.Navigation("Amount")
                        .IsRequired();
                });

            modelBuilder.Entity("Auctions.Domain.TimedAscendingAuction", b =>
                {
                    b.OwnsOne("Auctions.Domain.TimedAscendingOptions", "Options", b1 =>
                        {
                            b1.Property<long>("TimedAscendingAuctionAuctionId")
                                .HasColumnType("bigint");

                            b1.Property<long>("MinRaise")
                                .HasColumnType("bigint");

                            b1.Property<long>("ReservePrice")
                                .HasColumnType("bigint");

                            b1.Property<TimeSpan>("TimeFrame")
                                .HasColumnType("time");

                            b1.HasKey("TimedAscendingAuctionAuctionId");

                            b1.ToTable("Auctions");

                            b1.WithOwner()
                                .HasForeignKey("TimedAscendingAuctionAuctionId");
                        });

                    b.Navigation("Options")
                        .IsRequired();
                });

            modelBuilder.Entity("Auctions.Domain.TimedAscendingAuction", b =>
                {
                    b.Navigation("Bids");
                });
#pragma warning restore 612, 618
        }
    }
}
