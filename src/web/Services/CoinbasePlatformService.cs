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
    public class CoinbasePlatformService : IPlatformService
    {
        public string PlatformName => "COINBASE";

        private static readonly int[] s_granularitiesAllowed = new int[] { 86400 };
        private const string COINBASE_DATE_FORMAT = "yyyy-MM-dd";

        public string ApiUrl { get; private set; }

        public CoinbasePlatformService()
        {
            this.ApiUrl = "https://api.coinbase.com";
        }

        public async Task InitCurrencyPairs(CryptoInvestContext context)
        {
            var platform = context.Platforms.Include(p => p.PlatformRates).Include(p => p.PlatformCurrencyPairs)
                .SingleOrDefault(p => p.Name == this.PlatformName);

            if (platform.PlatformCurrencyPairs.Count() == 0)
            {
                // en dur pour le moment
                var products = new List<string>
                {
                    "BTC",
                    "ETH",
                    "LTC"
                };

                var targetCurrency = context.DefaultFiatCurrency;

                foreach (var sSourceCurrency in products)
                {
                    try
                    {
                        var sourceCurrency = context.Currencies.SingleOrDefault(c => c.Acronym == sSourceCurrency);

                        if (sourceCurrency == null || targetCurrency == null)
                            continue;

                        platform.PlatformCurrencyPairs.Add(new dal.models.PlatformCurrencyPair
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
            var product = $"{currencySource.Acronym}-{currencyTarget.Acronym}";

            if (!s_granularitiesAllowed.Contains(granularity))
                throw new Exception($"Granularity must be one of these values : {s_granularitiesAllowed.Select(g => g.ToString()).Aggregate((x, y) => x + ", " + y)}");

            var ratesResults = new List<PlatformRateData>();

            var results = new PlatformRateResult
            {
                CurrencySource = currencySource,
                CurrencyTarget = currencyTarget,
                Dates = datesToDownload,
            };

            try
            {
                foreach (var date in datesToDownload)
                {

                    var rateJson = await CallApi($"v2/prices/{product}/spot?date={date.ToString(COINBASE_DATE_FORMAT)}");

                    dynamic rate = (JToken)JsonConvert.DeserializeObject(rateJson);

                    ratesResults.Add(new PlatformRateData
                    {
                        Time = date,
                        Low = rate.data.amount,
                        High = rate.data.amount,
                        Open = rate.data.amount,
                        Close = rate.data.amount,
                        Volume = 0,
                    });

                    System.Threading.Thread.Sleep(500);

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
