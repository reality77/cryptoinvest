using dal.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Linq;

namespace dal
{
    public class CryptoInvestContext : DbContext
    {
        public CryptoInvestContext(DbContextOptions options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => new { x.Login })
                .IsUnique(true);

            modelBuilder.Entity<Currency>()
                .HasIndex(x => x.Name)
                .IsUnique(true);

            modelBuilder.Entity<Account>()
                .HasIndex(x => new { x.UserID, x.Name })
                .IsUnique(true);

            modelBuilder.Entity<Account>()
                .HasOne(x => x.Currency)
                .WithMany(x => x.Accounts)
                .HasForeignKey(x => x.CurrencyID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasIndex(x => new { x.UserID, x.Date })
                .IsUnique(false);

            modelBuilder.Entity<Transaction>()
                .HasIndex(x => x.SourceAccountID)
                .IsUnique(false);

            modelBuilder.Entity<Transaction>()
                .HasIndex(x => x.TargetAccountID)
                .IsUnique(false);

            modelBuilder.Entity<Transaction>()
                .HasOne(x => x.SourceAccount)
                .WithMany(x => x.SourceTransactions)
                .HasForeignKey(x => x.SourceAccountID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(x => x.TargetAccount)
                .WithMany(x => x.TargetTransactions)
                .HasForeignKey(x => x.TargetAccountID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>(pt => pt.Property(p => p.Date)
                .HasColumnType("date"));
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }

        public void SeedData()
        {
            this.Database.Migrate();

            if(this.Users.Count() == 0)
            {
                var defaultuser = new User
                {
                    Login = "default",
                };

                defaultuser.SetPassword("default");
                this.Users.Add(defaultuser);
                
                this.SaveChanges();
            }

            if(this.Currencies.Count() == 0)
            {
                this.Currencies.Add(new Currency
                {
                    Name = "Euro",
                    Acronym = "EUR",
                    IsFiat = true,
                });

                this.Currencies.Add(new Currency
                {
                    Name = "US Dollar",
                    Acronym = "USD",
                    IsFiat = true,
                });

                this.Currencies.Add(new Currency
                {
                    Name = "Bitcoin",
                    Acronym = "BTC",
                    IsFiat = false,
                });

                this.Currencies.Add(new Currency
                {
                    Name = "Ether",
                    Acronym = "ETH",
                    IsFiat = false,
                });

                this.SaveChanges();
            }

            if (this.Accounts.Count() == 0)
            {
                this.Accounts.Add(new Account
                {
                    Name = "Banque",
                    CurrencyID = 1,
                    UserID = 1
                });

                this.Accounts.Add(new Account
                {
                    Name = "Coinbase EUR",
                    CurrencyID = 1,
                    UserID = 1
                });

                this.Accounts.Add(new Account
                {
                    Name = "Coinbase BTC",
                    CurrencyID = 3,
                    UserID = 1
                });

                this.Accounts.Add(new Account
                {
                    Name = "Coinbase ETH",
                    CurrencyID = 4,
                    UserID = 1
                });

                this.Accounts.Add(new Account
                {
                    Name = "Kraken BTC",
                    CurrencyID = 3,
                    UserID = 1
                });

                this.Accounts.Add(new Account
                {
                    Name = "Kraken ETH",
                    CurrencyID = 4,
                    UserID = 1
                });

                this.SaveChanges();
            }
        }
    }
}
