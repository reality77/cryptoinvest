using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Services
{
    interface IPlatformService
    {
        string ApiUrl { get; }

        Task<PlatformRateResult> GetRates(dal.models.Currency currencySource, dal.models.Currency currencyTarget, DateTime start, DateTime end, int granularity);
    }

    public class PlatformRateResult
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public dal.models.Currency CurrencySource { get; set; }
        public dal.models.Currency CurrencyTarget { get; set; }

        public Dictionary<DateTime, PlatformRate> Rates { get; set; }
    }

    public class PlatformRate
    {
        public DateTime Time { get; set; }
        public decimal Low { get; set; }
        public decimal High { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }
}
