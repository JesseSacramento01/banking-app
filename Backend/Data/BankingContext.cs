using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public class BankingContext : DbContext
{
    public BankingContext(DbContextOptions<BankingContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
        .HasMany(u => u.Accounts)
        .WithOne(a => a.User)
        .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<Account>()
        .HasMany(a => a.Transactions)
        .WithOne(t => t.Account)
        .HasForeignKey(t => t.AccountId);
    }


}