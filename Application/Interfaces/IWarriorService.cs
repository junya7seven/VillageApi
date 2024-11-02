﻿using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IWarriorService
    {
        Task<IEnumerable<WarriorDTO>> GetAllAsync();
        Task<WarriorDTO> GetByIdAsync(int id);
        Task<WarriorDTO> CreateAsync(WarriorDTO warriorDTO);
        Task UpdateAsync(int id, WarriorDTO warriorDTO);
        Task DeleteAsync(int id);
    }
}
