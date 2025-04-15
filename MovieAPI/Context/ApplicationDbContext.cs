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
using MovieAPI.Domain.Qualities;
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
