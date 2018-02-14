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

            modelBuilder.Entity<Transaction>()
                .HasIndex(x => new { x.UserID, x.Date })
                .IsUnique(false);

            modelBuilder.Entity<Transaction>()
                .HasIndex(x => x.SourceAccountID)
                .IsUnique(false);

            modelBuilder.Entity<Transaction>()
                .HasIndex(x => x.TargetAccountID)
                .IsUnique(false);

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
        }
    }
}
