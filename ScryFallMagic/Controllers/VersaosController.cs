using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ScryFallMagic.Entities;
using ScryFallMagic.Data;

namespace ScryFallMagic.Controllers
{
    public class VersaosController : Controller
    {
        private readonly OracleDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public VersaosController(OracleDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        // GET: Versaos
        public async Task<IActionResult> Index()
        {
            if (!_memoryCache.TryGetValue("VersaosList", out List<Versao> versaosList))
            {
                versaosList = await _context.Versoes.ToListAsync();
                _memoryCache.Set("VersaosList", versaosList, TimeSpan.FromMinutes(10)); // Cache por 10 minutos
            }

            return versaosList != null ? View(versaosList) : Problem("Entity set 'OracleDbContext.Versoes' is null.");
        }

        // GET: Versaos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Versoes == null)
            {
                return NotFound();
            }

            if (!_memoryCache.TryGetValue($"VersaoDetails_{id}", out Versao versao))
            {
                versao = await _context.Versoes.FirstOrDefaultAsync(m => m.VersaoId == id);

                if (versao != null)
                {
                    _memoryCache.Set($"VersaoDetails_{id}", versao, TimeSpan.FromMinutes(10)); // Cache por 10 minutos
                }
            }

            if (versao == null)
            {
                return NotFound();
            }

            return View(versao);
        }

        // GET: Versaos/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VersaoId,Nome,Sigla,DataLancamento")] Versao versao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(versao);
                await _context.SaveChangesAsync();
                _memoryCache.Remove("VersaosList"); // Remova o cache da lista após a criação de uma nova versão
                return RedirectToAction(nameof(Index));
            }
            return View(versao);
        }

        // GET: Versaos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Versoes == null)
            {
                return NotFound();
            }

            var versao = await _context.Versoes.FindAsync(id);
            if (versao == null)
            {
                return NotFound();
            }
            return View(versao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VersaoId,Nome,Sigla,DataLancamento")] Versao versao)
        {
            if (id != versao.VersaoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(versao);
                    await _context.SaveChangesAsync();
                    _memoryCache.Remove($"VersaoDetails_{id}"); // Remova o cache após a edição
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VersaoExists(versao.VersaoId))
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
            return View(versao);
        }

        // GET: Versaos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Versoes == null)
            {
                return NotFound();
            }

            var versao = await _context.Versoes.FirstOrDefaultAsync(m => m.VersaoId == id);

            if (versao == null)
            {
                return NotFound();
            }

            return View(versao);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var versao = await _context.Versoes.FindAsync(id);
            if (versao != null)
            {
                _context.Versoes.Remove(versao);
                await _context.SaveChangesAsync();
                _memoryCache.Remove($"VersaoDetails_{id}"); // Remova o cache após a exclusão
                _memoryCache.Remove("VersaosList"); // Remova o cache da lista após a exclusão
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VersaoExists(int id)
        {
            return _context.Versoes.Any(e => e.VersaoId == id);
        }
    }
}