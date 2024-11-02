using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Interfaces
{
    public interface IWarriorRepository
    {
        Task<bool> Insert(Warrior warrior);
        Task<bool> Remove(Warrior warrior);
        Task<Warrior> GetByIdAsync(int id);
        Task<IEnumerable<Warrior>> GetAllAsync();
    }
}
