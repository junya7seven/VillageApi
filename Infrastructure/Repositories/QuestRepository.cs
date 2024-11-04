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
    public class QuestRepository : IQuestRepository
    {
        private readonly VillageContext _context;

        public QuestRepository(VillageContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Quest>> GetAllAsync() => await _context.Quests.ToListAsync();
        public async Task<Quest> GetByIdAsync(int id) => await _context.Quests.FindAsync(id);
        public async Task Insert(Quest quest) => await _context.Quests.AddAsync(quest);
        public void Remove(Quest quest) => _context.Remove(quest);
        public async Task Update(Quest quest) => _context.Quests.Update(quest);
    }
}
