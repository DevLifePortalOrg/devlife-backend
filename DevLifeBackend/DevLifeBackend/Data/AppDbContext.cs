// DevLife.Api/Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using DevLife.Domain.Entities;
using System;
using System.Collections.Generic;

namespace DevLife.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets for your PostgreSQL entities
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<CodeSnippetEntity> CodeSnippets { get; set; }
        public DbSet<CodeChallengeEntity> CodeChallenges { get; set; }
        public DbSet<ScoreEntity> Scores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserEntity
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Username).IsUnique(); // Ensure unique usernames
                entity.HasIndex(u => u.Email).IsUnique();     // Ensure unique emails
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("NOW()"); // Set default value for creation date
                entity.Property(u => u.Roles)
                      .HasColumnType("jsonb") // Store roles as JSONB array in PostgreSQL
                      .HasConversion(
                          v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                          v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>()
                      );
            });

            // Configure CodeSnippetEntity
            modelBuilder.Entity<CodeSnippetEntity>(entity =>
            {
                entity.HasKey(cs => cs.Id);
                entity.Property(cs => cs.SubmissionDate).HasDefaultValueSql("NOW()");
                entity.Property(cs => cs.Comments)
                      .HasColumnType("jsonb") // Store comments as JSONB array
                      .HasConversion(
                          v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                          v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>()
                      );

                // Define foreign key relationship with UserEntity
                entity.HasOne(cs => cs.User)
                      .WithMany(u => u.CodeSnippets)
                      .HasForeignKey(cs => cs.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Cascade delete snippets if user is deleted
            });

            // Configure CodeChallengeEntity
            modelBuilder.Entity<CodeChallengeEntity>(entity =>
            {
                entity.HasKey(cc => cc.Id);
                entity.Property(cc => cc.CreatedDate).HasDefaultValueSql("NOW()");
                entity.Property(cc => cc.Tags)
                      .HasColumnType("jsonb") // Store tags as JSONB array
                      .HasConversion(
                          v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                          v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>()
                      );
            });

            // Configure ScoreEntity
            modelBuilder.Entity<ScoreEntity>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.AchievedAt).HasDefaultValueSql("NOW()");

                // Define foreign key relationship with UserEntity
                entity.HasOne(s => s.User)
                      .WithMany(u => u.Scores)
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Cascade delete scores if user is deleted
            });

            // If you decide to link DatingProfileEntity to UserEntity via PostgreSQL:
            // This would be if DatingProfile was also stored in PostgreSQL and had a 1-to-1 relationship.
            // However, based on our discussion, DatingProfile is in MongoDB.
            // If it were in PostgreSQL:
            // modelBuilder.Entity<UserEntity>()
            //    .HasOne(u => u.DatingProfile)
            //    .WithOne(dp => dp.User)
            //    .HasForeignKey<DatingProfileEntity>(dp => dp.UserId);
        }
    }
}