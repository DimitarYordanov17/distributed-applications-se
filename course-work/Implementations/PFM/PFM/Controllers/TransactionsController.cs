using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PFM.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using PFM.ViewModels;

namespace PFM.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly PFMDbContext _context;

        public TransactionsController(PFMDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int categoryId = 0, int accountId = 0, string sortOrder = "")
        {
            var userId = GetUserId();

            var query = _context.Transactions
                .Where(t => t.UserId == userId);

            if (categoryId != 0)
                query = query.Where(t => t.CategoryId == categoryId);

            if (accountId != 0)
                query = query.Where(t => t.AccountId == accountId);

            query = query
                .Include(t => t.Account)
                .Include(t => t.Category);

            var transactions = await query.ToListAsync();

            if (sortOrder == "asc")
                transactions = transactions.OrderBy(t => t.Amount).ToList();
            else if (sortOrder == "desc")
                transactions = transactions.OrderByDescending(t => t.Amount).ToList();

            ViewBag.Categories = new SelectList(
                await _context.Categories.Where(c => c.UserId == userId).ToListAsync(),
                "Id", "Name");

            ViewBag.Accounts = new SelectList(
                await _context.Accounts.Where(a => a.UserId == userId).ToListAsync(),
                "Id", "Name");

            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedAccountId = accountId;
            ViewBag.SortOrder = sortOrder;

            return View(transactions);
        }


        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == GetUserId());

            if (transaction == null) return NotFound();

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            var userId = GetUserId();
            ViewData["AccountId"] = new SelectList(_context.Accounts.Where(a => a.UserId == userId), "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.UserId == userId), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,Date,Description,AccountId,CategoryId")] Transaction transaction)
        {
            var userId = GetUserId();
            transaction.UserId = userId;
            ModelState.Remove(nameof(transaction.UserId)); 


            if (!ModelState.IsValid)
            {

                ViewData["AccountId"] = new SelectList(_context.Accounts.Where(a => a.UserId == userId), "Id", "Name", transaction.AccountId);
                ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.UserId == userId), "Id", "Name", transaction.CategoryId);
                return View(transaction);
            }

            _context.Add(transaction);
            await _context.SaveChangesAsync();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == transaction.CategoryId && c.UserId == userId);
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == transaction.AccountId && a.UserId == userId);

            if (category == null || account == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (category.IsIncome)
                account.Balance += (double)transaction.Amount;
            else
                account.Balance -= (double)transaction.Amount;


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null || transaction.UserId != GetUserId()) return NotFound();

            var userId = GetUserId();
            ViewData["AccountId"] = new SelectList(_context.Accounts.Where(a => a.UserId == userId), "Id", "Name", transaction.AccountId);
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.UserId == userId), "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,Date,Description,AccountId,CategoryId")] Transaction transaction)
        {
            if (id != transaction.Id) return NotFound();

            var existing = await _context.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == GetUserId());

            if (existing == null) return NotFound();

            transaction.UserId = GetUserId();
            ModelState.Remove(nameof(transaction.UserId));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            var userId = GetUserId();
            ViewData["AccountId"] = new SelectList(_context.Accounts.Where(a => a.UserId == userId), "Id", "Name", transaction.AccountId);
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.UserId == userId), "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == GetUserId());

            if (transaction == null) return NotFound();

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == GetUserId());

            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id && e.UserId == GetUserId());
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
