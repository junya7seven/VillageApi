using Application.DTOs;
using Application.Interfaces;
using Entities.Interfaces;
using Entities.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    internal sealed class WarriorService : IWarriorService
    {
        private readonly IRepositoryManager _repositoryManager;
        public WarriorService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<IEnumerable<WarriorDTO>> GetAllAsync()
        {
            var warriors = await _repositoryManager.warriorRepository.GetAllAsync();
            var warriorDto = warriors.Adapt<IEnumerable<WarriorDTO>>();
            return warriorDto;
        }

        public async Task<WarriorDTO> GetByIdAsync(int id)
        {
            var warrior = await _repositoryManager.warriorRepository.GetByIdAsync(id);
            if (warrior == null) { }
            /// exception
            var warriorDto = warrior.Adapt<WarriorDTO>();
            return warriorDto;
        }

        public async Task<WarriorDTO> CreateAsync(WarriorDTO warriorDto)
        {
            var warrior = warriorDto.Adapt<Warrior>();
            await _repositoryManager.warriorRepository.Insert(warrior);
            return warrior.Adapt(warriorDto);
        }

        public async Task UpdateAsync(int id, WarriorDTO warriorDto)
        {
            var warrior = await _repositoryManager.warriorRepository.GetByIdAsync(id);
            if (warrior is null) { }
            /// exception
            warrior.FirstName = warriorDto.FirstName;
            warrior.NickName = warriorDto.NickName;
            warrior.EnrollmentDate = warriorDto.EnrollmentDate;
            await _repositoryManager.warriorRepository.Update(warrior);
            await _repositoryManager.unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var warrior = await _repositoryManager.warriorRepository.GetByIdAsync(id);
            if (warrior is null) { }
            /// exception
            _repositoryManager.warriorRepository.Remove(warrior);
            await _repositoryManager.unitOfWork.SaveChangesAsync();
        }
    }
}
