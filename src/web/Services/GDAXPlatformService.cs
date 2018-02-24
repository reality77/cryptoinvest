using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace web.Services
{
    public class GDAXPlatformService : IPlatformService
    {
        private static readonly int[] s_granularitiesAllowed = new int[] { 60, 300, 900, 3600, 21600, 86400 };
        private const string GDAX_DATE_FORMAT = "o";

        public string ApiUrl { get; private set; }

        public GDAXPlatformService()
        {
            this.ApiUrl = "https://api.gdax.com";
        }

        public async Task<PlatformRateResult> GetRates(dal.models.Currency currencySource, dal.models.Currency currencyTarget, DateTime start, DateTime end, int granularity)
        {
            var product = $"{currencySource.Acronym}-{currencyTarget.Acronym}";

            if (!s_granularitiesAllowed.Contains(granularity))
                throw new Exception($"Granularity must be one of these values : {s_granularitiesAllowed.Select(g => g.ToString()).Aggregate((x, y) => x + ", " + y)}");

            var ratesJson = await CallApi($"products/{product}/candles?start={start.ToString(GDAX_DATE_FORMAT)}&end={end.ToString(GDAX_DATE_FORMAT)}&granularity={granularity}");

            var rates = (JToken)JsonConvert.DeserializeObject(ratesJson);

            // 1519495200,675.95,676,675.97,675.95,63.75268696999999
            var results = new PlatformRateResult
            {
                CurrencySource = currencySource,
                CurrencyTarget = currencyTarget,
                Start = start,
                End = end,
                Rates = rates.Select(r =>
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
                }).ToDictionary(k => k.Time),
            };

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
