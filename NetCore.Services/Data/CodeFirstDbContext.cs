
using Microsoft.EntityFrameworkCore;
using NetCore.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Services.Data
{
    // 2. Fluent API
    // inheritance
    // CodeFirstDbContext -
    // DbContext -

    public class CodeFirstDbContext : DbContext
    {
        // constructor inheritance
        public CodeFirstDbContext(DbContextOptions<CodeFirstDbContext> options) : base(options)
        {

        }
        // DB table list 
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserRolesByUser> UserRolesByUsers { get; set; }

        // method inheritance
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 4 jobs
            // Database table name
            modelBuilder.Entity<User>().ToTable(name: "User");
            modelBuilder.Entity<UserRole>().ToTable(name: "UserRole");
            modelBuilder.Entity<UserRolesByUser>().ToTable(name: "UserRoelsByUser");

            // complex key
            modelBuilder.Entity<UserRolesByUser>().HasKey(c => new { c.UserId, c.RoleId });
            // column normal
            modelBuilder.Entity<User>( e =>
                {
                    e.Property(c => c.IsMembershipWithrawn).HasDefaultValue(value: false);
            });
            // index 
            modelBuilder.Entity<User>().HasIndex(c => new { c.UserEmail}).IsUnique(unique:true);


        }
    }
}
