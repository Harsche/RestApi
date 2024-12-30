using System;
using GameApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User{Id= 1, Username="Ocoloso", CreateDate = new DateTime(2024, 12, 30)},
            new User{Id= 2, Username="Biloso", CreateDate = new DateTime(2024, 9, 11)},
            new User{Id= 3, Username="Ocoscocosco", CreateDate = new DateTime(2024, 1, 11)}
        );
    }

}
