using dal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Controllers
{
    public class ControllerBase : Controller
    {
        protected CryptoInvestContext _context;

        protected void SetViewBagCurrencies(bool includeFiat = true, bool includeCrypto = true)
        {
            var currencies = _context.Currencies.AsQueryable();

            if (!includeCrypto)
                currencies = currencies.Where(c => c.IsFiat);
            if (!includeFiat)
                currencies = currencies.Where(c => !c.IsFiat);

            ViewBag.Currencies = currencies;
        }

        protected void SetViewBagAccounts()
        {
            var accs = _context.Accounts.AsQueryable();

            ViewBag.Accounts = accs.OrderBy(a => a.Name);
        }
    }
}
