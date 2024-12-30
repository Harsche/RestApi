using System;
using Microsoft.EntityFrameworkCore;

namespace GameApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

}
