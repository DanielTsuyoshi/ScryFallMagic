using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScryFallMagic.Data;
using ScryFallMagic.Entities;

namespace ScryFallMagic.Controllers
{
    public class ColecoesController : Controller
    {
        private readonly OracleDbContext _context;

        public ColecoesController(OracleDbContext context)
        {
            _context = context;
        }

        // GET: Colecoes
        public async Task<IActionResult> Index()
        {
              return _context.Colecao != null ? 
                          View(await _context.Colecao.ToListAsync()) :
                          Problem("Entity set 'OracleDbContext.Colecao'  is null.");
        }

        // GET: Colecoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Colecao == null)
            {
                return NotFound();
            }

            var colecao = await _context.Colecao
                .FirstOrDefaultAsync(m => m.Id == id);
            if (colecao == null)
            {
                return NotFound();
            }

            return View(colecao);
        }

        // GET: Colecoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Colecoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,DataLancamento,Idioma")] Colecao colecao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(colecao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(colecao);
        }

        // GET: Colecoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Colecao == null)
            {
                return NotFound();
            }

            var colecao = await _context.Colecao.FindAsync(id);
            if (colecao == null)
            {
                return NotFound();
            }
            return View(colecao);
        }

        // POST: Colecoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,DataLancamento,Idioma")] Colecao colecao)
        {
            if (id != colecao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(colecao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ColecaoExists(colecao.Id))
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
            return View(colecao);
        }

        // GET: Colecoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Colecao == null)
            {
                return NotFound();
            }

            var colecao = await _context.Colecao
                .FirstOrDefaultAsync(m => m.Id == id);
            if (colecao == null)
            {
                return NotFound();
            }

            return View(colecao);
        }

        // POST: Colecoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Colecao == null)
            {
                return Problem("Entity set 'OracleDbContext.Colecao'  is null.");
            }
            var colecao = await _context.Colecao.FindAsync(id);
            if (colecao != null)
            {
                _context.Colecao.Remove(colecao);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ColecaoExists(int id)
        {
          return (_context.Colecao?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
