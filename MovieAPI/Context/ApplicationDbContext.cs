using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MovieAPI.Domain.Categories;
using MovieAPI.Domain.Directors;
using MovieAPI.Domain.Genres;
using MovieAPI.Domain.Movies;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml.Linq;
using MovieAPI.Domain.Actors;
using MovieAPI.Domain.Comments;
using MovieAPI.Domain.Qualities;
using MovieAPI.Domain.Reviews;
using MovieAPI.Domain.Users;

namespace MovieAPI.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<Category> Categories { get; set; } 
        public DbSet<MovieCategory> MovieCategories { get; set; }
        public DbSet<Quality> Qualities { get; set; } 
        
        public DbSet<MovieQuality> MovieQualities { get; set; }
        
        public DbSet<Actor> Actors { get; set; } 
        
        public DbSet<MovieActor> MovieActors { get; set; }

        public DbSet<User> Users { get; set; }
        
        public DbSet<Comment> Comments { get; set; }
        
        public DbSet<CommentReaction> CommentReactions { get; set; }

        public DbSet<Review> Reviews { get; set; }
        
        public DbSet<UserMovieWatch> UserMovieWatches { get; set; }
        
        public DbSet<ReviewReaction> ReviewReactions { get; set; }
        
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieGenre>().HasKey(mg => new { mg.MovieId, mg.GenreId });

            modelBuilder.Entity<MovieCategory>().HasKey(mc => new { mc.MovieId, mc.CategoryId });

            modelBuilder.Entity<MovieQuality>().HasKey(mq => new { mq.MovieId, mq.QualityId });

            modelBuilder.Entity<MovieActor>().HasKey(ma => new { ma.MovieId, ma.ActorId });

            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Director)
                .WithMany(d => d.Movies)
                .HasForeignKey(m => m.DirectorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Movie)
                .WithMany(m => m.Comments)
                .HasForeignKey(c => c.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.QuotedComment)
                .WithMany()
                .HasForeignKey(c => c.QuotedCommentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CommentReaction>()
                .HasKey(cr => cr.CommentReactionId);

            modelBuilder.Entity<CommentReaction>()
                .HasOne(cr => cr.User)
                .WithMany(u => u.CommentReactions)
                .HasForeignKey(cr => cr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommentReaction>()
                .HasOne(cr => cr.Comment)
                .WithMany(c => c.Reactions)
                .HasForeignKey(cr => cr.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasMany(c => c.LikedByUsers)
                .WithMany(u => u.LikedComments) // assuming you have this on User
                .UsingEntity(j => j.ToTable("CommentLikes"));

            modelBuilder.Entity<Comment>()
                .HasMany(c => c.DislikedByUsers)
                .WithMany(u => u.DislikedComments) // assuming you have this on User
                .UsingEntity(j => j.ToTable("CommentDislikes"));

            modelBuilder.Entity<User>()
                .HasMany(u => u.LikedComments)
                .WithMany(c => c.LikedByUsers)
                .UsingEntity(j => j.ToTable("CommentLikes"));

            modelBuilder.Entity<User>()
                .HasMany(u => u.DislikedComments)
                .WithMany(c => c.DislikedByUsers)
                .UsingEntity(j => j.ToTable("CommentDislikes"));

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MovieId);
            
            modelBuilder.Entity<UserMovieWatch>()
                .HasIndex(w => new { w.UserId, w.MovieId, w.WatchedAt });

            modelBuilder.Entity<ReviewReaction>()
                .HasOne(rr => rr.User)
                .WithMany(u => u.Reactions)
                .HasForeignKey(rr => rr.UserId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<ReviewReaction>()
                .HasOne(rr => rr.Review)
                .WithMany(r => r.Reactions)
                .HasForeignKey(rr => rr.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<ReviewReaction>()
                .HasIndex(rr => new { rr.UserId, rr.ReviewId })
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.SubscriptionPlan)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
    public class AppDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseNpgsql("Host=localhost;Database=movie_db;Username=postgres;Password=root",
                options => options.EnableRetryOnFailure(3));
            return new ApplicationDbContext(builder.Options);
        }
    }
}
