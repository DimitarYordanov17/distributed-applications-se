using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFM.Models;
using Microsoft.AspNetCore.Authorization;

namespace PFM.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly PFMDbContext _context;

        public AccountsController(PFMDbContext context)
        {
            _context = context;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();
            return View(accounts);
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetUserId();
            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (account == null) return NotFound();

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account account)
        {
            account.UserId = GetUserId();
            ModelState.Remove(nameof(account.UserId));

            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(account);
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetUserId();
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (account == null) return NotFound();

            return View(account);
        }

        // POST: Accounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,Balance,CreatedDate,Description")] Account account)
        {
            if (id != account.Id) return NotFound();

            var userId = GetUserId();
            var existing = await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (existing == null) return NotFound();

            account.UserId = userId;
            ModelState.Remove(nameof(account.UserId));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id, userId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(account);
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetUserId();
            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (account == null) return NotFound();

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id, string userId)
        {
            return _context.Accounts.Any(e => e.Id == id && e.UserId == userId);
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
