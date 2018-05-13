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
    public class KrakenPlatformService : IPlatformService
    {
        public string PlatformName => "KRAKEN";

        private static readonly int[] s_granularitiesAllowed = new int[] { 1, 5, 15, 30, 60, 240, 1440, 10080, 21600 };
        private static readonly DateTime UNIX_ORIGIN = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public string ApiUrl { get; private set; }

        public KrakenPlatformService()
        {
            this.ApiUrl = "https://api.kraken.com/0/";
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
            granularity = granularity / 60; // granularity in minutes for Kraken API

            var product = $"{currencySource.Acronym}{currencyTarget.Acronym}";

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
                var startDate = datesToDownload.Min();
                var endDate = datesToDownload.Max();
                int nextFrom = DateTimeToUnixTimestamp(startDate.AddDays(-1));
                int endTo = DateTimeToUnixTimestamp(endDate);
                bool firstCall = true;

                while (nextFrom <= endTo)
                {
                    if (firstCall)
                        firstCall = false;
                    else
                        System.Threading.Thread.Sleep(500);

                    var rateJson = await CallApi($"public/OHLC?pair={product}&interval={granularity}&since={nextFrom}");

                    dynamic ratesData = JObject.Parse(rateJson);

                    //var errors = ratesData.Value("error");

                    JObject rates = ratesData.result;

                    if (rates == null)
                    {
                        Log.Error("RetrieveRates for {PlatformName} error : {Message}", this.PlatformName, ratesData);
                        break;
                    }
                    else
                    {
                        foreach (JArray rate in rates.First.Values())
                        {
                            var currentDate = UnixTimeStampToDateTime(Convert.ToInt32(rate[0])).Date;

                            if (datesToDownload.Contains(currentDate))
                            {
                                ratesResults.Add(new PlatformRateData
                                {
                                    Time = currentDate,
                                    Open = Convert.ToDecimal(rate[1]),
                                    High = Convert.ToDecimal(rate[2]),
                                    Low = Convert.ToDecimal(rate[3]),
                                    Close = Convert.ToDecimal(rate[4]),
                                    Volume = Convert.ToDecimal(rate[6]),
                                });
                            }
                        }

                        nextFrom = rates.Value<int>("last");
                    }
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

        public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            return UNIX_ORIGIN.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        public static int DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (int)(dateTime - UNIX_ORIGIN).TotalSeconds;
        }
    }
}
