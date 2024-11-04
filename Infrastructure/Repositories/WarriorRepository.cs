using Entities.Interfaces;
using Entities.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class WarriorRepository : IWarriorRepository
    {
        private readonly VillageContext _context;

        public WarriorRepository(VillageContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Warrior>> GetAllAsync() => await _context.Warriors.ToListAsync();
        public async Task<Warrior> GetByIdAsync(int id) => await _context.Warriors.FindAsync(id);
        public async Task Insert(Warrior warrior) => await _context.Warriors.AddAsync(warrior);
        public void Remove(Warrior warrior) => _context.Remove(warrior);
        public async Task Update(Warrior warrior) => _context.Warriors.Update(warrior);

    }
}
