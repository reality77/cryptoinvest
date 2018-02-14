using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace dal
{
    public class CryptoInvestContextFactory : IDesignTimeDbContextFactory<CryptoInvestContext>
    {
        public const string CONNECTION_STRING = "User ID=cryptoinvest;Password={1};Host={0};Port=5432;Database=cryptoinvest;Pooling=true;";

        public CryptoInvestContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CryptoInvestContext>();
            builder.UseNpgsql(GetConnectionString());

            var context = new CryptoInvestContext(builder.Options);
            return context;
        }

        public static string GetConnectionString()
        {
            var servername = Environment.GetEnvironmentVariable("APP_DB_SERVER");
            var passwordname = Environment.GetEnvironmentVariable("APP_DB_PASSWORD");

            if (string.IsNullOrEmpty(servername))
                servername = "localhost";

            if (string.IsNullOrEmpty(passwordname))
                passwordname = "cryptoinvest";

            return string.Format(CONNECTION_STRING, servername, passwordname);
        }
    }
}

