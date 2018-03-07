using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Services
{
    public interface IPlatformService
    {
        string PlatformName { get; }

        string ApiUrl { get; }

        Task InitCurrencyPairs(dal.CryptoInvestContext context);

        Task<PlatformRate> RetrieveDayRate(dal.models.Currency currencySource, dal.models.Currency currencyTarget, DateTime day);

        Task<PlatformRateResult> RetrieveRates(dal.models.Currency currencySource, dal.models.Currency currencyTarget, DateTime start, DateTime end, int granularity);
    }
}
