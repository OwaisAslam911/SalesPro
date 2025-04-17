using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SalesPro.Models;

public partial class SalesProContext : DbContext
{
    public SalesProContext()
    {
    }

    public SalesProContext(DbContextOptions<SalesProContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
