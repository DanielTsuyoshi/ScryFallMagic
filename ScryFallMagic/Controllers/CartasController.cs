using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ScryFallMagic.Entities;
using ScryFallMagic.Data;

namespace ScryFallMagic.Controllers
{
    public class CartasController : Controller
    {
        private readonly OracleDbContext _context;
        private readonly IMemoryCache _memoryCache;


        public CartasController(OracleDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;

        }

        // GET: Cartas
        public async Task<IActionResult> Index()
        {
            // Tente obter os dados da lista de cache
            if (!_memoryCache.TryGetValue("CartasList", out List<Carta> cartasList))
            {
                // Se não estiver em cache, busque no banco de dados
                cartasList = await _context.Cartas
                    .Include(c => c.Colecao)
                    .Include(c => c.Precos)
                    .Include(c => c.Versao)
                    .ToListAsync();

                // Armazene os dados em cache por um período de tempo
                _memoryCache.Set("CartasList", cartasList, TimeSpan.FromMinutes(10)); // Cache por 10 minutos
            }

            return View(cartasList);
        }

        // GET: Cartas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cartas == null)
            {
                return NotFound();
            }

            // Tente obter a carta do cache
            if (!_memoryCache.TryGetValue($"CartaDetails_{id}", out Carta carta))
            {
                // Se não estiver em cache, busque no banco de dados
                carta = await _context.Cartas
                    .Include(c => c.Colecao)
                    .Include(c => c.Precos)
                    .Include(c => c.Versao)
                    .FirstOrDefaultAsync(m => m.CartaId == id);

                if (carta != null)
                {
                    // Armazene a carta em cache por um período de tempo
                    _memoryCache.Set($"CartaDetails_{id}", carta, TimeSpan.FromMinutes(10)); // Cache por 10 minutos
                }
            }

            if (carta == null)
            {
                return NotFound();
            }

            return View(carta);
        }

        // GET: Cartas/Create
        public IActionResult Create()
        {
            // Tente obter os dados do SelectList do cache
            if (!_memoryCache.TryGetValue("CreateSelectLists", out (IEnumerable<SelectListItem> ColecaoList, IEnumerable<SelectListItem> PrecoCartaList, IEnumerable<SelectListItem> VersaoList) selectLists))
            {
                // Se não estiver em cache, busque os dados no banco de dados
                var ColecaoList = _context.Colecoes.Select(c => new SelectListItem { Value = c.ColecaoId.ToString(), Text = c.Nome }).ToList();
                var VersaoList = _context.Versoes.Select(v => new SelectListItem { Value = v.VersaoId.ToString(), Text = v.Nome }).ToList();

                // Armazene os SelectList em cache por um período de tempo
                selectLists = (ColecaoList, VersaoList);
                _memoryCache.Set("CreateSelectLists", selectLists, TimeSpan.FromMinutes(10)); // Cache por 10 minutos
            }

            ViewData["ColecaoId"] = new SelectList(selectLists.ColecaoList, "Value", "Text");
            ViewData["VersaoId"] = new SelectList(selectLists.VersaoList, "Value", "Text");
            return View();
        }

        // POST: Cartas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Cartas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartaId,Nome,CustoDeMana,Linguagem,Tipo,Raridade,Edicao,Texto,TextoNarrativo,Poder,Resistencia,Artista,SuperTipo,SubTipo,VersaoId,ColecaoId,PrecoCartaId")] Carta carta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Tente obter os SelectListItems do cache
            if (!_memoryCache.TryGetValue("CreateSelectListItems", out (IEnumerable<SelectListItem> ColecaoList, IEnumerable<SelectListItem> PrecoCartaList, IEnumerable<SelectListItem> VersaoList) selectListItems))
            {
                // Se não estiver em cache, busque os dados no banco de dados
                var ColecaoList = await _context.Colecoes.Select(c => new SelectListItem { Value = c.ColecaoId.ToString(), Text = c.Nome }).ToListAsync();
                var VersaoList = await _context.Versoes.Select(v => new SelectListItem { Value = v.VersaoId.ToString(), Text = v.Nome }).ToListAsync();

                // Armazene os SelectListItems em cache por um período de tempo
                selectListItems = (ColecaoList, VersaoList);
                _memoryCache.Set("CreateSelectListItems", selectListItems, TimeSpan.FromMinutes(10)); // Cache por 10 minutos
            }

            ViewData["ColecaoId"] = new SelectList(selectListItems.ColecaoList, "Value", "Text", carta.ColecaoId);
            ViewData["VersaoId"] = new SelectList(selectListItems.VersaoList, "Value", "Text", carta.VersaoId);
            return View(carta);
        }

        // GET: Cartas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cartas == null)
            {
                return NotFound();
            }

            // Tente obter a carta do cache
            if (!_memoryCache.TryGetValue($"Carta-{id}", out Carta carta))
            {
                // Se não estiver em cache, busque no banco de dados
                carta = await _context.Cartas.FindAsync(id);

                // Armazene a carta em cache por um período de tempo
                if (carta != null)
                {
                    _memoryCache.Set($"Carta-{id}", carta, TimeSpan.FromMinutes(10)); // Cache por 10 minutos
                }
            }

            if (carta == null)
            {
                return NotFound();
            }

            ViewData["ColecaoId"] = new SelectList(_context.Colecoes, "ColecaoId", "Nome", carta.ColecaoId);
            ViewData["VersaoId"] = new SelectList(_context.Versoes, "VersaoId", "Nome", carta.VersaoId);
            return View(carta);
        }

        // POST: Cartas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartaId,Nome,CustoDeMana,Linguagem,Tipo,Raridade,Edicao,Texto,TextoNarrativo,Poder,Resistencia,Artista,SuperTipo,SubTipo,VersaoId,ColecaoId,PrecoCartaId")] Carta carta)
        {
            if (id != carta.CartaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualize a carta no banco de dados
                    _context.Update(carta);
                    await _context.SaveChangesAsync();

                    // Remova a carta do cache para que os dados em cache sejam atualizados
                    _memoryCache.Remove($"Carta-{id}");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartaExists(carta.CartaId))
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

            ViewData["ColecaoId"] = new SelectList(_context.Colecoes, "ColecaoId", "Nome", carta.ColecaoId);
            ViewData["VersaoId"] = new SelectList(_context.Versoes, "VersaoId", "Nome", carta.VersaoId);
            return View(carta);
        }

        // GET: Cartas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cartas == null)
            {
                return NotFound();
            }

            // Tente obter a carta do cache
            if (!_memoryCache.TryGetValue($"Carta-{id}", out Carta carta))
            {
                // Se não estiver em cache, busque no banco de dados
                carta = await _context.Cartas
                    .Include(c => c.Colecao)
                    .Include(c => c.Precos)
                    .Include(c => c.Versao)
                    .FirstOrDefaultAsync(m => m.CartaId == id);

                // Armazene a carta em cache por um período de tempo
                if (carta != null)
                {
                    _memoryCache.Set($"Carta-{id}", carta, TimeSpan.FromMinutes(10)); // Cache por 10 minutos
                }
            }

            if (carta == null)
            {
                return NotFound();
            }

            return View(carta);
        }


        // POST: Cartas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cartas == null)
            {
                return Problem("Entity set 'OracleDbContext.Cartas'  is null.");
            }

            // Tente obter a carta do cache
            if (_memoryCache.TryGetValue($"Carta-{id}", out Carta carta))
            {
                _context.Cartas.Remove(carta);
                await _context.SaveChangesAsync();

                // Remova a carta do cache após excluí-la
                _memoryCache.Remove($"Carta-{id}");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CartaExists(int id)
        {
            if (_context.Cartas == null)
            {
                return false;
            }

            // Verifique se a carta está no cache
            if (_memoryCache.TryGetValue($"Carta-{id}", out _))
            {
                return true;
            }

            return _context.Cartas.Any(e => e.CartaId == id);
        }
    }
}