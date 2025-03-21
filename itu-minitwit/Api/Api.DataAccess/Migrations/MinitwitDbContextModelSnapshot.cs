﻿// <auto-generated />
using Api.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Api.DataAccess.Migrations
{
    [DbContext(typeof(MinitwitDbContext))]
    partial class MinitwitDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2");

            modelBuilder.Entity("Api.DataAccess.Models.Follower", b =>
                {
                    b.Property<int>("WhoId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("who_id");

                    b.Property<int>("WhomId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("whom_id");

                    b.HasKey("WhoId", "WhomId");

                    b.HasIndex("WhoId", "WhomId")
                        .IsUnique();

                    b.ToTable("follower", (string)null);
                });

            modelBuilder.Entity("Api.DataAccess.Models.LatestProcessedSimAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Latest")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("LatestProcessedSimActions");
                });

            modelBuilder.Entity("Api.DataAccess.Models.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("message_id");

                    b.Property<int>("AuthorId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("author_id");

                    b.Property<int?>("Flagged")
                        .HasColumnType("INTEGER")
                        .HasColumnName("flagged");

                    b.Property<int?>("PubDate")
                        .HasColumnType("INTEGER")
                        .HasColumnName("pub_date");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("string")
                        .HasColumnName("text");

                    b.HasKey("MessageId");

                    b.HasIndex("AuthorId", "Flagged", "PubDate")
                        .IsDescending(false, false, true);

                    b.ToTable("message", (string)null);
                });

            modelBuilder.Entity("Api.DataAccess.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("user_id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("string")
                        .HasColumnName("email");

                    b.Property<string>("PwHash")
                        .IsRequired()
                        .HasColumnType("string")
                        .HasColumnName("pw_hash");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("string")
                        .HasColumnName("username");

                    b.HasKey("UserId");

                    b.HasIndex(new[] { "Username" }, "IX_user_username")
                        .IsUnique();

                    b.ToTable("user", (string)null);
                });

            modelBuilder.Entity("Api.DataAccess.Models.Message", b =>
                {
                    b.HasOne("Api.DataAccess.Models.User", "Author")
                        .WithMany("Messages")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("Api.DataAccess.Models.User", b =>
                {
                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
