using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<EnrollmentDTO>> GetAllAsync();
        Task<EnrollmentDTO> GetByIdAsync(int id);
        Task<EnrollmentDTO> CreateAsync(EnrollmentDTO enrollmentDTO);
        Task UpdateAsync(int id, EnrollmentDTO enrollmentDTO);
        Task DeleteAsync(int id);
    }
}
