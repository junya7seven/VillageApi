using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<bool> Insert(Enrollment enrollment);
        void Remove(Enrollment enrollment);
        Task<Enrollment> GetByIdAsync(int id);
        Task<IEnumerable<Enrollment>> GetAllAsync();
    }
}
