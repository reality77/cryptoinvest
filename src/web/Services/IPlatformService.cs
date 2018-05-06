using dal;
using dal.models;
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

        Task InitCurrencyPairs(CryptoInvestContext context);

        Task<PlatformRateResult> RetrieveRates(CryptoInvestContext context, Currency currencySource, Currency currencyTarget, IEnumerable<DateTime> datesToDownload, int granularity);
    }
}