using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Services
{
    public class RatesSynchronization
    {
        class RateKey
        {
            public int? PlatformID { get; set; }
            public int CurrencyID { get; set; }
            public DateTime Date { get; set; }
        }

        /// <summary>
        /// Synchronizes the missing rates for the transactions dates (from the default FIAT currency)
        /// </summary>
        public static void AutoSync(dal.CryptoInvestContext context)
        {
            List<Task> tasks = new List<Task>();

            if (context.Platforms.Count() > 0 && PlatformServices.Instance.Count() == 0)
            {
                context.Platforms.ToList().ForEach(p =>
                {
                    string serviceKey = p.Name.ToUpper();

                    //Set up for each platform the associated web service client (example: GDAX => GDAXPlatformService)
                    IPlatformService service = null;

                    switch (serviceKey)
                    {
                        case "GDAX":
                            service = new GDAXPlatformService();
                            break;
                        case "COINBASE":
                            service = new CoinbasePlatformService();
                            break;
                    }

                    if (service != null)
                    {
                        PlatformServices.Instance.AddService(serviceKey, service);
                        tasks.Add(service.InitCurrencyPairs(context));
                    }
                });
            }

            Task.WaitAll(tasks.ToArray());

            foreach (var platformId in context.Platforms.Include(p => p.PlatformCurrencyPairs).Select(p => p.ID))
            {
                // Each platform will be synchronized asynchronously
                Task.Run(() => SynchronizePlatformAsync(platformId));
            }
        }

        private static void SynchronizePlatformAsync(int platformId)
        {
            var factory = new dal.CryptoInvestContextFactory();

            var contextLocal = factory.CreateDbContext(null);
            contextLocal.InitDefaultCurrency();

            var platform = contextLocal.Platforms
                .Include(p => p.PlatformCurrencyPairs)
                .Single(p => p.ID == platformId);

            if (platform.PlatformCurrencyPairs != null && platform.PlatformCurrencyPairs.Count() > 0)
            {
                foreach (var pairs in platform.PlatformCurrencyPairs)
                {
                    bool? takeSource = null;

                    if (pairs.SourceCurrencyID == contextLocal.DefaultFiatCurrency.ID)
                        takeSource = false;
                    else if (pairs.TargetCurrencyID == contextLocal.DefaultFiatCurrency.ID)
                        takeSource = true;

                    if (takeSource != null)
                    {
                        IEnumerable<dal.models.Transaction> transactions;

                        // Retrieving transaction on the current platform for the current currency pair
                        if (takeSource == true)
                        {
                            transactions = contextLocal.Transactions.Include(t => t.SourceAccount).ThenInclude(a => a.Currency)
                                .Where(t => t.SourceAccount.PlatformID == platform.ID && t.SourceAccount.CurrencyID == pairs.SourceCurrencyID);
                        }
                        else
                        {
                            transactions = contextLocal.Transactions.Include(t => t.TargetAccount).ThenInclude(a => a.Currency)
                                .Where(t => t.SourceAccount.PlatformID == platform.ID && t.TargetAccount.CurrencyID == pairs.TargetCurrencyID);
                        }

                        if (transactions.Count() > 0)
                        {
                            // Listing transactions dates
                            var dates = transactions.Select(t => t.Date).Distinct().ToList();

                            var startDate = contextLocal.Transactions
                                .OrderBy(t => new { t.Date })
                                .First()
                                .Date;

                            // Listing already downloaded rate dates 
                            var currentRateDates = contextLocal.PlatformRates
                                .Where(r => r.PlatformID == pairs.PlatformID && r.SourceCurrencyID == pairs.SourceCurrencyID && r.TargetCurrencyID == pairs.TargetCurrencyID)
                                .Select(r => r.Date);

                            var service = PlatformServices.Instance.Get(platform.Name.ToUpper());

                            // We keep the dates which were not already downloaded
                            dates.RemoveAll(d => currentRateDates.Contains(d));

                            /*
                             * TODO : gérer dates  qui ne retournent pas de données
                             * VOIR PB GDAX BTC/EUR
                             * */

                            if (dates.Count() > 0)
                            {
                                // Rates lookup
                                var results = service.RetrieveRates(contextLocal, pairs.SourceCurrency, pairs.TargetCurrency, dates, 86400).Result;

                                // Insert results in DB
                                var rates = results.Rates.Select(r => new dal.models.PlatformRate()
                                {
                                    PlatformID = platform.ID,
                                    SourceCurrencyID = pairs.SourceCurrencyID,
                                    TargetCurrencyID = pairs.TargetCurrencyID,
                                    Open = r.Value.Open,
                                    Close = r.Value.Close,
                                    High = r.Value.High,
                                    Low = r.Value.Low,
                                    Date = r.Value.Time,
                                    Volume = r.Value.Volume,
                                    RateSet = null,
                                });

                                contextLocal.PlatformRates.AddRange(rates);
                                contextLocal.SaveChanges();
                            }
                        }
                    }
                }
            }
        }
    }
}
