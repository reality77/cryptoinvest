using dal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PlatformRate> RetrieveDayRate(dal.models.Currency currencySource, dal.models.Currency currencyTarget, DateTime day)
        {
            var start = day.Date;
            var end = start.AddDays(1);

            var rates = await RetrieveRates(currencySource, currencyTarget, start, end, 86400);
            return rates.Rates[start];
        }

        public async Task<PlatformRateResult> RetrieveRates(dal.models.Currency currencySource, dal.models.Currency currencyTarget, DateTime start, DateTime end, int granularity)
        {
            var product = $"{currencySource.Acronym}-{currencyTarget.Acronym}";

            if (!s_granularitiesAllowed.Contains(granularity))
                throw new Exception($"Granularity must be one of these values : {s_granularitiesAllowed.Select(g => g.ToString()).Aggregate((x, y) => x + ", " + y)}");

            var ratesResults = new List<PlatformRate>();

            var results = new PlatformRateResult
            {
                CurrencySource = currencySource,
                CurrencyTarget = currencyTarget,
                Start = start,
                End = end,
            };

            DateTime startLoop = start;
            DateTime endLoop = startLoop.AddDays(GDAX_MAX_RESPONSES);

            if (end < endLoop)
                endLoop = end;

            while (endLoop < end)
            {
                try
                {
                    var ratesJson = await CallApi($"products/{product}/candles?start={startLoop.ToString(GDAX_DATE_FORMAT)}&end={endLoop.ToString(GDAX_DATE_FORMAT)}&granularity={granularity}");

                    var rates = (JToken)JsonConvert.DeserializeObject(ratesJson);

                    ratesResults.AddRange(rates.Select(r =>
                    {
                        var arData = r.ToArray();

                        return new PlatformRate
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

                    if (end < endLoop)
                        endLoop = end;
                    else
                        System.Threading.Thread.Sleep(250);
                }
                catch(Exception ex)
                {

                }
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
