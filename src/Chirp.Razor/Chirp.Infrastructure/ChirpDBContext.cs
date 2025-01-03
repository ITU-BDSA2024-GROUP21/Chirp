﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace Chirp.Infrastructure;

public class ChirpDBContext : IdentityDbContext<ApplicationUser>
{
    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
    }
    
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<AuthorFollow> AuthorFollows { get; set; }
    public DbSet<Bio> Bios { get; set; }
    
    // This method configures relationships in our database model
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Define the many-to-many relationship
        modelBuilder.Entity<AuthorFollow>()
            .HasKey(af => new { af.FollowerId, af.FollowingId }); // Composite key

        modelBuilder.Entity<AuthorFollow>()
            .HasOne(af => af.Follower)
            .WithMany(a => a.Following)
            .HasForeignKey(af => af.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AuthorFollow>()
            .HasOne(af => af.Following)
            .WithMany(a => a.Followers)
            .HasForeignKey(af => af.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Bio>()
            .HasKey(b => new { b.BioId, b.AuthorId });
        
        modelBuilder.Entity<Bio>()
            .HasOne(b => b.Author)
            .WithOne(a => a.Bio)
            .HasForeignKey<Bio>(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
            
    }
    
}
