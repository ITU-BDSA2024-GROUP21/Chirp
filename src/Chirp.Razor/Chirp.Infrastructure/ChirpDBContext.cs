﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace Chirp.Infrastructure;

public class ChirpDBContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
    }

 
    
}