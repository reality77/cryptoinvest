using dal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using dal.models;
using Serilog;

namespace web.Services
{
    public class GDAXPlatformService : IPlatformService
    {
        public string PlatformName => "GDAX";

        private static readonly int[] s_granularitiesAllowed = new int[] { 60, 300, 900, 3600, 21600, 86400 };
        private const string GDAX_DATE_FORMAT = "o";
        private const int GDAX_MAX_RESPONSES = 100;

        public string ApiUrl { get; private set; }

        public GDAXPlatformService()
        {
            this.ApiUrl = "https://api.gdax.com";
        }

        public async Task InitCurrencyPairs(CryptoInvestContext context)
        {
            var platform = context.Platforms.Include(p => p.PlatformRates).Include(p => p.PlatformCurrencyPairs)
                .SingleOrDefault(p => p.Name == this.PlatformName);

            if (platform.PlatformCurrencyPairs.Count() == 0)
            {
                var productsJson = await CallApi($"products");
                var products = (JToken)JsonConvert.DeserializeObject(productsJson);

                foreach (dynamic product in products)
                {
                    string sSourceCurrency = product.base_currency;
                    string sTargetCurrency = product.quote_currency;

                    try
                    {
                        var sourceCurrency = context.Currencies.SingleOrDefault(c => c.Acronym == sSourceCurrency);
                        var targetCurrency = context.Currencies.SingleOrDefault(c => c.Acronym == sTargetCurrency);

                        if (sourceCurrency == null || targetCurrency == null)
                            continue;

                        platform.PlatformCurrencyPairs.Add(new PlatformCurrencyPair
                        {
                            SourceCurrencyID = sourceCurrency.ID,
                            TargetCurrencyID = targetCurrency.ID,
                            PlatformID = platform.ID,
                        });
                    }
                    catch (Exception ex)
                    {
                        var error = ex.ToString();
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task<PlatformRateResult> RetrieveRates(CryptoInvestContext context, Currency currencySource, Currency currencyTarget, IEnumerable<DateTime> datesToDownload, int granularity)
        {
            // Pour GDAX, nous recherchons toutes les dates 
            var platform = context.Platforms.Include(p => p.PlatformRates).Include(p => p.PlatformCurrencyPairs)
                .SingleOrDefault(p => p.Name == this.PlatformName);

            var startDate = datesToDownload.Min();
            var endDate = datesToDownload.Max();

            var product = $"{currencySource.Acronym}-{currencyTarget.Acronym}";

            if (!s_granularitiesAllowed.Contains(granularity))
                throw new Exception($"Granularity must be one of these values : {s_granularitiesAllowed.Select(g => g.ToString()).Aggregate((x, y) => x + ", " + y)}");

            // Vue que le service GDAX recherche toutes les dates, on peut partir de la dernière date en base
            var lastDate = context.PlatformRates
                .Where(r => r.PlatformID == platform.ID && r.SourceCurrencyID == currencySource.ID && r.TargetCurrencyID == currencyTarget.ID)
                .OrderByDescending(r => r.Date)
                .FirstOrDefault(r => r.Date > startDate)?.Date;

            if (lastDate != null)
                startDate = lastDate.Value;

            var ratesResults = new List<PlatformRateData>();

            var results = new PlatformRateResult
            {
                CurrencySource = currencySource,
                CurrencyTarget = currencyTarget,
                Dates = datesToDownload,
            };

            if (startDate >= endDate)
                return results;

            DateTime startLoop = startDate;
            DateTime endLoop = startLoop.AddDays(GDAX_MAX_RESPONSES);

            if (endDate < endLoop)
                endLoop = endDate;

            try
            {
                while (endLoop < endDate)
                {
                    var ratesJson = await CallApi($"products/{product}/candles?start={startLoop.ToString(GDAX_DATE_FORMAT)}&end={endLoop.ToString(GDAX_DATE_FORMAT)}&granularity={granularity}");

                    var rates = (JToken)JsonConvert.DeserializeObject(ratesJson);

                    ratesResults.AddRange(rates.Select(r =>
                    {
                        var arData = r.ToArray();

                        return new PlatformRateData
                        {
                            Time = DateTimeExtensions.FromUnixTime(Convert.ToInt64(arData[0])),
                            Low = Convert.ToDecimal(arData[1]),
                            High = Convert.ToDecimal(arData[2]),
                            Open = Convert.ToDecimal(arData[3]),
                            Close = Convert.ToDecimal(arData[4]),
                            Volume = Convert.ToDecimal(arData[5]),
                        };
                    }));

                    startLoop = endLoop.AddDays(1);
                    endLoop = startLoop.AddDays(GDAX_MAX_RESPONSES);

                    if (endDate < endLoop)
                        endLoop = endDate;
                    else
                        System.Threading.Thread.Sleep(2000);    // pour éviter des erreurs 429 - Too Much Requests
                }
            }
            catch (Exception ex)
            {
                Log.Error("RetrieveRates for {PlatformName} error : {Message}", this.PlatformName, ex.Message);
            }

            results.Rates = ratesResults.ToDictionary(k => k.Time);            

            return results;
        }

        private async Task<string> CallApi(string relativeUrl)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.ApiUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Application");

                try
                {
                    return await client.GetStringAsync(relativeUrl);
                }
                catch(Exception ex)
                {
                    string message = ex.ToString();
                    throw;
                }
            }
        }
    }
}
