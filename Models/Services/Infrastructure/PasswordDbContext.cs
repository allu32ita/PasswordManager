﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PasswordManager.Models.Entities;

namespace PasswordManager.Models.Services.Infrastructure
{
    public partial class PasswordDbContext : DbContext
    {

        public PasswordDbContext(DbContextOptions<PasswordDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Passwords> Passwords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Passwords>(entity =>
            {
                entity.ToTable("Passwords");
                entity.HasKey(Passwords => Passwords.Id);
                entity.HasIndex(e => e.Id).IsUnique();
                entity.Property(Passwords => Passwords.RowVersion).IsRowVersion();
                //entity.Property(e => e.Id).ValueGeneratedNever();   questo dice che non deve usare la sua proprieta autoincrementale nel database
            });
        }
    }
}
