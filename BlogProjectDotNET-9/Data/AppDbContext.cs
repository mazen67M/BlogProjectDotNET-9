﻿using BlogProjectDotNET_9.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogProjectDotNET_9.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }
        public DbSet<Post> Posts{ get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category { Id =1 , Name="Technology", Description="Description 1"},
                new Category { Id =2 , Name="Health", Description="Description 2"},
                new Category { Id =3 , Name="Life Style", Description="Description 3"}
                );

            modelBuilder.Entity<Post>().HasData(
                new Post
                {
                    Id = 1,
                    Title = "Tech Post 1",
                    Content = "Content of Tech Post 1",
                    Author = "John Doe",
                    PublishedDate = new DateTime(2023, 1, 1), // Static date instead of DateTime.Now
                    CategoryId = 1,
                    FeatureImagePath = "tech_image.jpg", // Sample image path
                },
                new Post
                {
                    Id = 2,
                    Title = "Health Post 1",
                    Content = "Content of Health Post 1",
                    Author = "Jane Doe",
                    PublishedDate = new DateTime(2023, 1, 1), // Static date
                    CategoryId = 2,
                    FeatureImagePath = "health_image.jpg", // Sample image path
                },
                new Post
                {
                    Id = 3,
                    Title = "Lifestyle Post 1",
                    Content = "Content of Lifestyle Post 1",
                    Author = "Alex Smith",
                    PublishedDate = new DateTime(2023, 1, 1), // Static date
                    CategoryId = 3,
                    FeatureImagePath = "lifestyle_image.jpg", // Sample image path
                });
        }
    }
   
}
