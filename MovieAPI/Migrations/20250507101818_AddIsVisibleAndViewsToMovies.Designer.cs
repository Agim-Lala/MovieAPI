﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieAPI.Context;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MovieAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250507101818_AddIsVisibleAndViewsToMovies")]
    partial class AddIsVisibleAndViewsToMovies
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CommentUser", b =>
                {
                    b.Property<int>("LikedByUsersId")
                        .HasColumnType("integer");

                    b.Property<int>("LikedCommentsCommentId")
                        .HasColumnType("integer");

                    b.HasKey("LikedByUsersId", "LikedCommentsCommentId");

                    b.HasIndex("LikedCommentsCommentId");

                    b.ToTable("CommentLikes", (string)null);
                });

            modelBuilder.Entity("CommentUser1", b =>
                {
                    b.Property<int>("DislikedByUsersId")
                        .HasColumnType("integer");

                    b.Property<int>("DislikedCommentsCommentId")
                        .HasColumnType("integer");

                    b.HasKey("DislikedByUsersId", "DislikedCommentsCommentId");

                    b.HasIndex("DislikedCommentsCommentId");

                    b.ToTable("CommentDislikes", (string)null);
                });

            modelBuilder.Entity("MovieAPI.Domain.Actors.Actor", b =>
                {
                    b.Property<int>("ActorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ActorId"));

                    b.Property<string>("ActorName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("ActorId");

                    b.ToTable("Actors");
                });

            modelBuilder.Entity("MovieAPI.Domain.Actors.MovieActor", b =>
                {
                    b.Property<int>("MovieId")
                        .HasColumnType("integer");

                    b.Property<int>("ActorId")
                        .HasColumnType("integer");

                    b.HasKey("MovieId", "ActorId");

                    b.HasIndex("ActorId");

                    b.ToTable("MovieActors");
                });

            modelBuilder.Entity("MovieAPI.Domain.Categories.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CategoryId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("MovieAPI.Domain.Categories.MovieCategory", b =>
                {
                    b.Property<int>("MovieId")
                        .HasColumnType("integer");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.HasKey("MovieId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("MovieCategories");
                });

            modelBuilder.Entity("MovieAPI.Domain.Comments.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CommentId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DislikesCount")
                        .HasColumnType("integer");

                    b.Property<int>("LikesCount")
                        .HasColumnType("integer");

                    b.Property<int>("MovieId")
                        .HasColumnType("integer");

                    b.Property<int?>("ParentCommentId")
                        .HasColumnType("integer");

                    b.Property<int?>("QuotedCommentId")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("CommentId");

                    b.HasIndex("MovieId");

                    b.HasIndex("ParentCommentId");

                    b.HasIndex("QuotedCommentId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("MovieAPI.Domain.Comments.CommentReaction", b =>
                {
                    b.Property<int>("CommentReactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CommentReactionId"));

                    b.Property<int>("CommentId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsDislike")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsLike")
                        .HasColumnType("boolean");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("CommentReactionId");

                    b.HasIndex("CommentId");

                    b.HasIndex("UserId");

                    b.ToTable("CommentReactions");
                });

            modelBuilder.Entity("MovieAPI.Domain.Directors.Director", b =>
                {
                    b.Property<int>("DirectorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("DirectorId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("DirectorId");

                    b.ToTable("Directors");
                });

            modelBuilder.Entity("MovieAPI.Domain.Genres.Genre", b =>
                {
                    b.Property<int>("GenreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("GenreId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("GenreId");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("MovieAPI.Domain.Genres.MovieGenre", b =>
                {
                    b.Property<int>("MovieId")
                        .HasColumnType("integer");

                    b.Property<int>("GenreId")
                        .HasColumnType("integer");

                    b.HasKey("MovieId", "GenreId");

                    b.HasIndex("GenreId");

                    b.ToTable("MovieGenres");
                });

            modelBuilder.Entity("MovieAPI.Domain.Movies.Movie", b =>
                {
                    b.Property<int>("MovieId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MovieId"));

                    b.Property<DateTime>("AddedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("AverageRating")
                        .HasColumnType("double precision");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<int>("DirectorId")
                        .HasColumnType("integer");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsVisible")
                        .HasColumnType("boolean");

                    b.Property<int>("ReleaseYear")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("Views")
                        .HasColumnType("integer");

                    b.HasKey("MovieId");

                    b.HasIndex("DirectorId");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("MovieAPI.Domain.Qualities.MovieQuality", b =>
                {
                    b.Property<int>("MovieId")
                        .HasColumnType("integer");

                    b.Property<int>("QualityId")
                        .HasColumnType("integer");

                    b.HasKey("MovieId", "QualityId");

                    b.HasIndex("QualityId");

                    b.ToTable("MovieQualities");
                });

            modelBuilder.Entity("MovieAPI.Domain.Qualities.Quality", b =>
                {
                    b.Property<int>("QualityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("QualityId"));

                    b.Property<string>("QualityName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("QualityId");

                    b.ToTable("Qualities");
                });

            modelBuilder.Entity("MovieAPI.Domain.Reviews.Review", b =>
                {
                    b.Property<int>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ReviewId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MovieId")
                        .HasColumnType("integer");

                    b.Property<double>("Rating")
                        .HasColumnType("double precision");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("ReviewId");

                    b.HasIndex("MovieId");

                    b.HasIndex("UserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("MovieAPI.Domain.Users.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CommentUser", b =>
                {
                    b.HasOne("MovieAPI.Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("LikedByUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieAPI.Domain.Comments.Comment", null)
                        .WithMany()
                        .HasForeignKey("LikedCommentsCommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CommentUser1", b =>
                {
                    b.HasOne("MovieAPI.Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("DislikedByUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieAPI.Domain.Comments.Comment", null)
                        .WithMany()
                        .HasForeignKey("DislikedCommentsCommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MovieAPI.Domain.Actors.MovieActor", b =>
                {
                    b.HasOne("MovieAPI.Domain.Actors.Actor", "Actor")
                        .WithMany("MovieActors")
                        .HasForeignKey("ActorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieAPI.Domain.Movies.Movie", "Movie")
                        .WithMany("MovieActors")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Actor");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("MovieAPI.Domain.Categories.MovieCategory", b =>
                {
                    b.HasOne("MovieAPI.Domain.Categories.Category", "Category")
                        .WithMany("MovieCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieAPI.Domain.Movies.Movie", "Movie")
                        .WithMany("MovieCategories")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("MovieAPI.Domain.Comments.Comment", b =>
                {
                    b.HasOne("MovieAPI.Domain.Movies.Movie", "Movie")
                        .WithMany("Comments")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieAPI.Domain.Comments.Comment", "ParentComment")
                        .WithMany("Replies")
                        .HasForeignKey("ParentCommentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MovieAPI.Domain.Comments.Comment", "QuotedComment")
                        .WithMany()
                        .HasForeignKey("QuotedCommentId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("MovieAPI.Domain.Users.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");

                    b.Navigation("ParentComment");

                    b.Navigation("QuotedComment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MovieAPI.Domain.Comments.CommentReaction", b =>
                {
                    b.HasOne("MovieAPI.Domain.Comments.Comment", "Comment")
                        .WithMany("Reactions")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieAPI.Domain.Users.User", "User")
                        .WithMany("CommentReactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Comment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MovieAPI.Domain.Genres.MovieGenre", b =>
                {
                    b.HasOne("MovieAPI.Domain.Genres.Genre", "Genre")
                        .WithMany("MovieGenres")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieAPI.Domain.Movies.Movie", "Movie")
                        .WithMany("MovieGenres")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Genre");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("MovieAPI.Domain.Movies.Movie", b =>
                {
                    b.HasOne("MovieAPI.Domain.Directors.Director", "Director")
                        .WithMany("Movies")
                        .HasForeignKey("DirectorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Director");
                });

            modelBuilder.Entity("MovieAPI.Domain.Qualities.MovieQuality", b =>
                {
                    b.HasOne("MovieAPI.Domain.Movies.Movie", "Movie")
                        .WithMany("MovieQualities")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieAPI.Domain.Qualities.Quality", "Quality")
                        .WithMany("MovieQualities")
                        .HasForeignKey("QualityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");

                    b.Navigation("Quality");
                });

            modelBuilder.Entity("MovieAPI.Domain.Reviews.Review", b =>
                {
                    b.HasOne("MovieAPI.Domain.Movies.Movie", "Movie")
                        .WithMany("Reviews")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieAPI.Domain.Users.User", "User")
                        .WithMany("Reviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MovieAPI.Domain.Actors.Actor", b =>
                {
                    b.Navigation("MovieActors");
                });

            modelBuilder.Entity("MovieAPI.Domain.Categories.Category", b =>
                {
                    b.Navigation("MovieCategories");
                });

            modelBuilder.Entity("MovieAPI.Domain.Comments.Comment", b =>
                {
                    b.Navigation("Reactions");

                    b.Navigation("Replies");
                });

            modelBuilder.Entity("MovieAPI.Domain.Directors.Director", b =>
                {
                    b.Navigation("Movies");
                });

            modelBuilder.Entity("MovieAPI.Domain.Genres.Genre", b =>
                {
                    b.Navigation("MovieGenres");
                });

            modelBuilder.Entity("MovieAPI.Domain.Movies.Movie", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("MovieActors");

                    b.Navigation("MovieCategories");

                    b.Navigation("MovieGenres");

                    b.Navigation("MovieQualities");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("MovieAPI.Domain.Qualities.Quality", b =>
                {
                    b.Navigation("MovieQualities");
                });

            modelBuilder.Entity("MovieAPI.Domain.Users.User", b =>
                {
                    b.Navigation("CommentReactions");

                    b.Navigation("Comments");

                    b.Navigation("Reviews");
                });
#pragma warning restore 612, 618
        }
    }
}
