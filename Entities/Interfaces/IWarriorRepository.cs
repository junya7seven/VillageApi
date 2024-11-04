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
        Task Insert(Warrior warrior);
        void Remove(Warrior warrior);
        Task Update(Warrior warrior);
        Task<Warrior> GetByIdAsync(int id);
        Task<IEnumerable<Warrior>> GetAllAsync();
    }
}
