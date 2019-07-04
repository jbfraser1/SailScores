﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SailScores.Database;

namespace SailScores.Database.Migrations
{
    [DbContext(typeof(SailScoresContext))]
    [Migration("20190704033023_ScoringTweaks")]
    partial class ScoringTweaks
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SailScores.Database.Entities.BoatClass", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ClubId");

                    b.Property<string>("Description")
                        .HasMaxLength(2000);

                    b.Property<string>("Name")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.ToTable("BoatClasses");
                });

            modelBuilder.Entity("SailScores.Database.Entities.Club", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("DefaultScoringSystemId");

                    b.Property<string>("Description");

                    b.Property<string>("Initials")
                        .HasMaxLength(10);

                    b.Property<bool>("IsHidden");

                    b.Property<string>("Locale");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("DefaultScoringSystemId");

                    b.ToTable("Clubs");
                });

            modelBuilder.Entity("SailScores.Database.Entities.Competitor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AlternativeSailNumber")
                        .HasMaxLength(20);

                    b.Property<Guid>("BoatClassId");

                    b.Property<string>("BoatName")
                        .HasMaxLength(200);

                    b.Property<Guid>("ClubId");

                    b.Property<bool?>("IsActive");

                    b.Property<string>("Name")
                        .HasMaxLength(200);

                    b.Property<string>("Notes")
                        .HasMaxLength(2000);

                    b.Property<string>("SailNumber")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.HasIndex("BoatClassId");

                    b.HasIndex("ClubId");

                    b.ToTable("Competitors");
                });

            modelBuilder.Entity("SailScores.Database.Entities.CompetitorFleet", b =>
                {
                    b.Property<Guid>("CompetitorId");

                    b.Property<Guid>("FleetId");

                    b.HasKey("CompetitorId", "FleetId");

                    b.HasIndex("FleetId");

                    b.ToTable("CompetitorFleet");
                });

            modelBuilder.Entity("SailScores.Database.Entities.File", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<byte[]>("FileContents");

                    b.Property<DateTime?>("ImportedTime");

                    b.HasKey("Id");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("SailScores.Database.Entities.Fleet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ClubId");

                    b.Property<string>("Description")
                        .HasMaxLength(2000);

                    b.Property<int>("FleetType");

                    b.Property<bool>("IsHidden");

                    b.Property<string>("Name")
                        .HasMaxLength(200);

                    b.Property<string>("ShortName")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.ToTable("Fleets");
                });

            modelBuilder.Entity("SailScores.Database.Entities.FleetBoatClass", b =>
                {
                    b.Property<Guid>("FleetId");

                    b.Property<Guid>("BoatClassId");

                    b.HasKey("FleetId", "BoatClassId");

                    b.HasIndex("BoatClassId");

                    b.ToTable("FleetBoatClass");
                });

            modelBuilder.Entity("SailScores.Database.Entities.HistoricalResults", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<bool>("IsCurrent");

                    b.Property<string>("Results");

                    b.Property<Guid>("SeriesId");

                    b.HasKey("Id");

                    b.HasIndex("SeriesId");

                    b.ToTable("HistoricalResults");
                });

            modelBuilder.Entity("SailScores.Database.Entities.Race", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ClubId");

                    b.Property<DateTime?>("Date");

                    b.Property<string>("Description")
                        .HasMaxLength(1000);

                    b.Property<Guid?>("FleetId");

                    b.Property<string>("Name")
                        .HasMaxLength(200);

                    b.Property<int>("Order");

                    b.Property<string>("State")
                        .HasMaxLength(30);

                    b.Property<string>("TrackingUrl")
                        .HasMaxLength(500);

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnName("UpdatedDateUtc");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.HasIndex("FleetId");

                    b.ToTable("Races");
                });

            modelBuilder.Entity("SailScores.Database.Entities.Score", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<Guid>("CompetitorId");

                    b.Property<int?>("Place");

                    b.Property<Guid>("RaceId");

                    b.HasKey("Id");

                    b.HasIndex("CompetitorId");

                    b.HasIndex("RaceId");

                    b.ToTable("Scores");
                });

            modelBuilder.Entity("SailScores.Database.Entities.ScoreCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool?>("AdjustOtherScores");

                    b.Property<bool?>("CameToStart");

                    b.Property<string>("Description")
                        .HasMaxLength(1000);

                    b.Property<bool?>("Discardable");

                    b.Property<bool?>("Finished");

                    b.Property<string>("Formula")
                        .HasMaxLength(100);

                    b.Property<int?>("FormulaValue");

                    b.Property<string>("Name")
                        .HasMaxLength(20);

                    b.Property<bool?>("PreserveResult");

                    b.Property<string>("ScoreLike");

                    b.Property<Guid>("ScoringSystemId");

                    b.Property<bool?>("Started");

                    b.HasKey("Id");

                    b.HasIndex("ScoringSystemId");

                    b.ToTable("ScoreCodes");
                });

            modelBuilder.Entity("SailScores.Database.Entities.ScoringSystem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("ClubId");

                    b.Property<string>("DiscardPattern");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<Guid?>("ParentSystemId");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.HasIndex("ParentSystemId");

                    b.ToTable("ScoringSystems");
                });

            modelBuilder.Entity("SailScores.Database.Entities.Season", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ClubId");

                    b.Property<DateTime>("End");

                    b.Property<string>("Name")
                        .HasMaxLength(200);

                    b.Property<DateTime>("Start");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("SailScores.Database.Entities.Series", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ClubId");

                    b.Property<string>("Description")
                        .HasMaxLength(2000);

                    b.Property<bool?>("IsImportantSeries");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<Guid?>("ScoringSystemId");

                    b.Property<Guid>("SeasonId");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnName("UpdatedDateUtc");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.HasIndex("ScoringSystemId");

                    b.HasIndex("SeasonId");

                    b.ToTable("Series");
                });

            modelBuilder.Entity("SailScores.Database.Entities.SeriesRace", b =>
                {
                    b.Property<Guid>("SeriesId");

                    b.Property<Guid>("RaceId");

                    b.HasKey("SeriesId", "RaceId");

                    b.HasIndex("RaceId");

                    b.ToTable("SeriesRace");
                });

            modelBuilder.Entity("SailScores.Database.Entities.UserClubPermission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("CanEditAllClubs");

                    b.Property<Guid?>("ClubId");

                    b.Property<string>("UserEmail")
                        .HasMaxLength(254);

                    b.HasKey("Id");

                    b.ToTable("UserPermissions");
                });

            modelBuilder.Entity("SailScores.Database.Entities.BoatClass", b =>
                {
                    b.HasOne("SailScores.Database.Entities.Club", "Club")
                        .WithMany("BoatClasses")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SailScores.Database.Entities.Club", b =>
                {
                    b.HasOne("SailScores.Database.Entities.ScoringSystem", "DefaultScoringSystem")
                        .WithMany()
                        .HasForeignKey("DefaultScoringSystemId");
                });

            modelBuilder.Entity("SailScores.Database.Entities.Competitor", b =>
                {
                    b.HasOne("SailScores.Database.Entities.BoatClass", "BoatClass")
                        .WithMany()
                        .HasForeignKey("BoatClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SailScores.Database.Entities.Club", "Club")
                        .WithMany("Competitors")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SailScores.Database.Entities.CompetitorFleet", b =>
                {
                    b.HasOne("SailScores.Database.Entities.Competitor", "Competitor")
                        .WithMany("CompetitorFleets")
                        .HasForeignKey("CompetitorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SailScores.Database.Entities.Fleet", "Fleet")
                        .WithMany("CompetitorFleets")
                        .HasForeignKey("FleetId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SailScores.Database.Entities.Fleet", b =>
                {
                    b.HasOne("SailScores.Database.Entities.Club", "Club")
                        .WithMany("Fleets")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SailScores.Database.Entities.FleetBoatClass", b =>
                {
                    b.HasOne("SailScores.Database.Entities.BoatClass", "BoatClass")
                        .WithMany()
                        .HasForeignKey("BoatClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SailScores.Database.Entities.Fleet", "Fleet")
                        .WithMany("FleetBoatClasses")
                        .HasForeignKey("FleetId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SailScores.Database.Entities.HistoricalResults", b =>
                {
                    b.HasOne("SailScores.Database.Entities.Series", "Series")
                        .WithMany()
                        .HasForeignKey("SeriesId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SailScores.Database.Entities.Race", b =>
                {
                    b.HasOne("SailScores.Database.Entities.Club", "Club")
                        .WithMany("Races")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SailScores.Database.Entities.Fleet", "Fleet")
                        .WithMany()
                        .HasForeignKey("FleetId");
                });

            modelBuilder.Entity("SailScores.Database.Entities.Score", b =>
                {
                    b.HasOne("SailScores.Database.Entities.Competitor", "Competitor")
                        .WithMany("Scores")
                        .HasForeignKey("CompetitorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SailScores.Database.Entities.Race", "Race")
                        .WithMany("Scores")
                        .HasForeignKey("RaceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SailScores.Database.Entities.ScoreCode", b =>
                {
                    b.HasOne("SailScores.Database.Entities.ScoringSystem")
                        .WithMany("ScoreCodes")
                        .HasForeignKey("ScoringSystemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SailScores.Database.Entities.ScoringSystem", b =>
                {
                    b.HasOne("SailScores.Database.Entities.Club")
                        .WithMany("ScoringSystems")
                        .HasForeignKey("ClubId");

                    b.HasOne("SailScores.Database.Entities.ScoringSystem", "ParentSystem")
                        .WithMany()
                        .HasForeignKey("ParentSystemId");
                });

            modelBuilder.Entity("SailScores.Database.Entities.Season", b =>
                {
                    b.HasOne("SailScores.Database.Entities.Club", "Club")
                        .WithMany("Seasons")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SailScores.Database.Entities.Series", b =>
                {
                    b.HasOne("SailScores.Database.Entities.Club")
                        .WithMany("Series")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SailScores.Database.Entities.ScoringSystem", "ScoringSystem")
                        .WithMany()
                        .HasForeignKey("ScoringSystemId");

                    b.HasOne("SailScores.Database.Entities.Season", "Season")
                        .WithMany("Series")
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SailScores.Database.Entities.SeriesRace", b =>
                {
                    b.HasOne("SailScores.Database.Entities.Race", "Race")
                        .WithMany("SeriesRaces")
                        .HasForeignKey("RaceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SailScores.Database.Entities.Series", "Series")
                        .WithMany("RaceSeries")
                        .HasForeignKey("SeriesId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
