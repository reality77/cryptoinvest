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
                    }

                    if (service != null)
                    {
                        PlatformServices.Instance.AddService(serviceKey, service);
                        tasks.Add(service.InitCurrencyPairs(context));
                    }
                });
            }

            Task.WaitAll(tasks.ToArray());

            foreach (var platform in context.Platforms.Include(p => p.PlatformCurrencyPairs))
            {
                if(platform.PlatformCurrencyPairs != null && platform.PlatformCurrencyPairs.Count() > 0)
                {
                    foreach (var pairs in platform.PlatformCurrencyPairs)
                    {
                        bool? takeSource = null;

                        if (pairs.SourceCurrencyID == context.DefaultFiatCurrency.ID)
                            takeSource = false;
                        else if(pairs.TargetCurrencyID == context.DefaultFiatCurrency.ID)
                            takeSource = true;

                        if(takeSource != null)
                        {
                            IEnumerable<dal.models.Transaction> transactions;

                            if (takeSource == true)
                            {
                                transactions = context.Transactions.Include(t => t.SourceAccount).ThenInclude(a => a.Currency)
                                    .Where(t => t.SourceAccount.PlatformID == platform.ID && t.SourceAccount.CurrencyID == pairs.SourceCurrencyID);
                            }
                            else
                            {
                                transactions = context.Transactions.Include(t => t.TargetAccount).ThenInclude(a => a.Currency)
                                    .Where(t => t.SourceAccount.PlatformID == platform.ID && t.TargetAccount.CurrencyID == pairs.TargetCurrencyID);
                            }

                            if (transactions.Count() > 0)
                            {
                                var startDate = context.Transactions
                                    .OrderBy(t => new { t.Date })
                                    .First()
                                    .Date;

                                var endDate = DateTime.Today.AddDays(-1);

                                if (startDate != null)
                                {
                                    var lastDate = context.PlatformRates
                                        .Where(r => r.PlatformID == pairs.PlatformID && r.SourceCurrencyID == pairs.SourceCurrencyID && r.TargetCurrencyID == pairs.TargetCurrencyID)
                                        .OrderByDescending(r => r.Date)
                                        .FirstOrDefault(r => r.Date > startDate)?.Date;

                                    if (lastDate != null)
                                        startDate = lastDate.Value;
                                }

                                var service = PlatformServices.Instance.Get(platform.Name.ToUpper());
                                var results = service.RetrieveRates(pairs.SourceCurrency, pairs.TargetCurrency, startDate, endDate, 86400).Result;

                                // TODO : Insert results in DB
                            }
                        }
                    }
                }
            }
        }

        public static void SynchronizeDays(DateTime start, DateTime end, dal.models.Currency ccySource, dal.models.Currency ccyTarget, dal.CryptoInvestContext context)
        {
            start = start.Date;
            end = end.Date;

            var tasks = new List<Task>();

            foreach (var service in PlatformServices.Instance)
            {
                tasks.Add(service.RetrieveRates(ccySource, ccyTarget, start, end, 86400));
            }

            Parallel.ForEach<Task>(tasks, task =>
            {
                task.Start();
            });
        }
    }
}
