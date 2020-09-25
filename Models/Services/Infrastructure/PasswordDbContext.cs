using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PasswordManager.Models.Entities;

namespace PasswordManager.Models.Services.Infrastructure
{
    public partial class PasswordDbContext : IdentityDbContext<ApplicationUser>
    {

        public PasswordDbContext(DbContextOptions<PasswordDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Passwords> Passwords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");
            modelBuilder.Entity<Passwords>(entity =>
            {
                entity.ToTable("Passwords");
                entity.HasKey(Passwords => Passwords.Id);
                entity.HasIndex(e => e.Id).IsUnique();
                entity.Property(Passwords => Passwords.RowVersion).IsRowVersion();

                entity.HasOne(Passwords => Passwords.Tab_AspNetUsers)
                            .WithMany(var_User => var_User.Tab_Passwords)
                            .HasForeignKey(Passwords => Passwords.FkUtente);
                //entity.Property(e => e.Id).ValueGeneratedNever();   questo dice che non deve usare la sua proprieta autoincrementale nel database
            });
        }
    }
}
