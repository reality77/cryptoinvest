using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dal;
using web.Models;
using dal.models;
using Microsoft.EntityFrameworkCore;

namespace web.Controllers
{
    public class TransactionsController : ControllerBase
    {
        public TransactionsController(CryptoInvestContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var filter = new TransactionsFilterModel();

            return View(filter);
        }

        [HttpGet]
        public IActionResult Filter()
        {
            var transactions = _context.Transactions
                .Include(t => t.SourceAccount).ThenInclude(a => a.Currency)
                .Include(t => t.TargetAccount).ThenInclude(a => a.Currency)
                .Where(t => t.UserID == 1)
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.ID);

            return View(transactions);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            SetViewBagAccounts();
            return View(new Transaction
            {
                UserID = 1,
                Date = DateTime.Today,
            });
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserID,Type,Date,SourceAccountID,TargetAccountID,SourceAmount,SourceFees,TargetAmount,TargetFees,Caption")] Transaction transaction, bool createNew = false)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();

                if(!createNew)
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction(nameof(Create));
            }

            SetViewBagAccounts();
            return View(transaction);
        }
    }
}