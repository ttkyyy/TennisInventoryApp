using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TennisInventoryApp.Models;

namespace TennisInventoryApp.Data.Ef
{
    public class EfPlayerRepository
    {
        private readonly ApplicationDbContext _context;

        public EfPlayerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<int> AddAsync(Player player)
        {
            await _context.Players.AddAsync(player);
            return await _context.SaveChangesAsync();
        }

        // READ - все игроки
        public async Task<List<Player>> GetAllAsync()
        {
            return await _context.Players
                .OrderByDescending(p => p.Rating)
                .ToListAsync();
        }

        // READ - по ID
        public async Task<Player> GetByIdAsync(Guid id)
        {
            return await _context.Players.FindAsync(id);
        }

        // UPDATE
        public async Task<int> UpdateAsync(Player player)
        {
            _context.Players.Update(player);
            return await _context.SaveChangesAsync();
        }

        // DELETE
        public async Task<int> DeleteAsync(Guid id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }
    }
}