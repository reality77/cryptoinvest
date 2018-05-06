using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dal;
using dal.models;
using web.Models;

namespace web.Controllers
{
    public class AccountsController : ControllerBase
    {
        public AccountsController(CryptoInvestContext context)
        {
            _context = context;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Accounts.Include(a => a.Currency)
                .Include(a => a.SourceTransactions).ThenInclude(t => t.TargetAccount).ThenInclude(a => a.Currency)
                .Include(a => a.TargetTransactions).ThenInclude(t => t.SourceAccount).ThenInclude(a => a.Currency)
                .ToListAsync());
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.Include(a => a.Currency)
                .Include(a => a.SourceTransactions).ThenInclude(t => t.TargetAccount).ThenInclude(a => a.Currency)
                .Include(a => a.TargetTransactions).ThenInclude(t => t.SourceAccount).ThenInclude(a => a.Currency)
                .SingleOrDefaultAsync(m => m.ID == id);

            var allTransactions = account.SourceTransactions.Select(t => new TransactionWithDirection
            {
                CurrencyAcronym = account.Currency.Acronym,
                Direction = EDirection.To,
                OtherAccount = t.TargetAccount,
                OtherCurrencyAcronym = t.TargetAccount?.Currency.Acronym,
                Transaction = t,
            }).ToList();

            allTransactions.AddRange(account.TargetTransactions.Select(t => new TransactionWithDirection
            {
                CurrencyAcronym = account.Currency.Acronym,
                Direction = EDirection.From,
                OtherAccount = t.SourceAccount,
                OtherCurrencyAcronym = t.TargetAccount?.Currency.Acronym,
                Transaction = t,
            }));

            var model = new AccountDetailsModel
            {
                DefaultCurrency = _context.DefaultFiatCurrency,
                TotalNetDebit = account.GetDebitSum(netAmount: true),
                TotalNetCredit = account.GetCreditSum(netAmount: true),
                TotalGrossDebit = account.GetDebitSum(netAmount: false),
                TotalGrossCredit = account.GetCreditSum(netAmount: false),
                Account = account,
                AllTransactions = allTransactions.OrderByDescending(t => t.Transaction.Date),
            };

            model.GrossBalance = model.TotalGrossCredit - model.TotalGrossDebit;

            if (account == null)
                return NotFound();

            return View(model);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            SetViewBagCurrencies();

            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserID,Name,Caption,CurrencyID")] Account account)
        {
            if (ModelState.IsValid)
            {

                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            SetViewBagCurrencies();

            return View(account);
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.SingleOrDefaultAsync(m => m.ID == id);
            if (account == null)
            {
                return NotFound();
            }

            SetViewBagCurrencies();

            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,UserID,Name,Caption,CurrencyID")] Account account)
        {
            if (id != account.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            SetViewBagCurrencies();

            return View(account);
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .SingleOrDefaultAsync(m => m.ID == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(m => m.ID == id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.ID == id);
        }
    }
}
