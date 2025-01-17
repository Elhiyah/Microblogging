﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebMicroblogging.Models;

#nullable disable

namespace WebMicroblogging.Migrations
{
    [DbContext(typeof(MicrobloggingContext))]
    [Migration("20241001180033_UpdateFechaNacimientoType")]
    partial class UpdateFechaNacimientoType
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WebMicroblogging.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(280)
                        .HasColumnType("nvarchar(280)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("TweetId")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id")
                        .HasName("PK__Comments__3214EC077C441B67");

                    b.HasIndex("TweetId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("WebMicroblogging.Models.Follow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<Guid>("FollowerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id")
                        .HasName("PK__Follows__3214EC07A93EEBB1");

                    b.HasIndex("FollowerId");

                    b.HasIndex("UserId");

                    b.ToTable("Follows");
                });

            modelBuilder.Entity("WebMicroblogging.Models.Like", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("TweetId")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id")
                        .HasName("PK__Likes__3214EC073D7DE6EC");

                    b.HasIndex("TweetId");

                    b.HasIndex("UserId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("WebMicroblogging.Models.Tweet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(980)
                        .HasColumnType("nvarchar(980)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id")
                        .HasName("PK__Tweets__3214EC076C7F3612");

                    b.HasIndex("UserId");

                    b.ToTable("Tweets");
                });

            modelBuilder.Entity("WebMicroblogging.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("(newid())");

                    b.Property<bool?>("Activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("FechaNacimiento")
                        .HasColumnType("datetime2");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id")
                        .HasName("PK__Users__3214EC0719610809");

                    b.HasIndex(new[] { "Email" }, "UQ__Users__A9D10534449B1C77")
                        .IsUnique()
                        .HasFilter("[Email] IS NOT NULL");

                    b.HasIndex(new[] { "UserName" }, "UQ__Users__C9F2845611D43C9C")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebMicroblogging.Models.Comment", b =>
                {
                    b.HasOne("WebMicroblogging.Models.Tweet", "Tweet")
                        .WithMany("Comments")
                        .HasForeignKey("TweetId")
                        .IsRequired()
                        .HasConstraintName("FK__Comments__TweetI__2F10007B");

                    b.HasOne("WebMicroblogging.Models.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK__Comments__UserId__300424B4");

                    b.Navigation("Tweet");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebMicroblogging.Models.Follow", b =>
                {
                    b.HasOne("WebMicroblogging.Models.User", "Follower")
                        .WithMany("FollowFollowers")
                        .HasForeignKey("FollowerId")
                        .IsRequired()
                        .HasConstraintName("FK__Follows__Followe__37A5467C");

                    b.HasOne("WebMicroblogging.Models.User", "User")
                        .WithMany("FollowUsers")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK__Follows__UserId__36B12243");

                    b.Navigation("Follower");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebMicroblogging.Models.Like", b =>
                {
                    b.HasOne("WebMicroblogging.Models.Tweet", "Tweet")
                        .WithMany("Likes")
                        .HasForeignKey("TweetId")
                        .IsRequired()
                        .HasConstraintName("FK__Likes__TweetId__32E0915F");

                    b.HasOne("WebMicroblogging.Models.User", "User")
                        .WithMany("Likes")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK__Likes__UserId__33D4B598");

                    b.Navigation("Tweet");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebMicroblogging.Models.Tweet", b =>
                {
                    b.HasOne("WebMicroblogging.Models.User", "User")
                        .WithMany("Tweets")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK__Tweets__UserId__2B3F6F97");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebMicroblogging.Models.Tweet", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Likes");
                });

            modelBuilder.Entity("WebMicroblogging.Models.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("FollowFollowers");

                    b.Navigation("FollowUsers");

                    b.Navigation("Likes");

                    b.Navigation("Tweets");
                });
#pragma warning restore 612, 618
        }
    }
}
