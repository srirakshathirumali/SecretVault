using Microsoft.EntityFrameworkCore;
using SecretVault.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // ── User ──────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Email)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.HasIndex(e => e.Email)
                      .IsUnique();
                entity.Property(e => e.PasswordHash)
                      .IsRequired();
                entity.Property(e => e.Role)
                      .HasConversion<string>();
                entity.Property(e => e.CreatedOn)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // ── Account ───────────────────────────────────
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AccountNumber)
                      .IsRequired()
                      .HasMaxLength(10);
                entity.HasIndex(e => e.AccountNumber)
                      .IsUnique();
                entity.Property(e => e.Balance)
                      .HasColumnType("decimal(18,2)")
                      .HasDefaultValue(0);
                entity.Property(e => e.Type)
                      .HasConversion<string>();
                entity.Property(e => e.CreatedOn)
                      .HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Accounts)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Transaction ───────────────────────────────
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount)
                      .HasColumnType("decimal(18,2)");
                entity.Property(e => e.BalanceAfter)
                      .HasColumnType("decimal(18,2)");
                entity.Property(e => e.Type)
                      .HasConversion<string>();
                entity.Property(e => e.Description)
                      .HasMaxLength(200);
                entity.Property(e => e.Timestamp)
                      .HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(e => e.Account)
                      .WithMany(a => a.Transactions)
                      .HasForeignKey(e => e.AccountId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── AuditLog ──────────────────────────────────
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Action)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.EntityType)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.IPAddress)
                      .HasMaxLength(45);
                entity.Property(e => e.Timestamp)
                      .HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(e => e.User)
                      .WithMany(u => u.AuditLogs)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
