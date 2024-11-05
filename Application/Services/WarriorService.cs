using Application.DTOs;
using Application.Interfaces;
using Entities.Interfaces;
using Entities.Models;
using Mapster;

namespace Application.Services
{
    internal sealed class WarriorService : IWarriorService
    {
        private readonly IRepositoryManager _repositoryManager;
        public WarriorService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<IEnumerable<Warrior>> GetAllAsync()
        {

            var warriors = await _repositoryManager.warriorRepository.GetAllAsync();
            if (!warriors.Any())
                return null;
            return warriors;

        }

        public async Task<Warrior> GetByIdAsync(int id)
        {

            var warrior = await _repositoryManager.warriorRepository.GetByIdAsync(id);
            if (warrior == null)
                return null;
            return warrior;

        }

        public async Task<WarriorDTO> CreateAsync(WarriorDTO warriorDto)
        {
            try
            {
                var warrior = warriorDto.Adapt<Warrior>();
                await _repositoryManager.warriorRepository.Insert(warrior);
                await _repositoryManager.unitOfWork.SaveChangesAsync();
                return warrior.Adapt(warriorDto);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdateAsync(int id, WarriorDTO warriorDto)
        {
            try
            {
                var warrior = await _repositoryManager.warriorRepository.GetByIdAsync(id);
                if (warrior is null)
                    //return null;
                if (warriorDto.FirstName != warrior.FirstName && warriorDto.FirstName != null)
                    warrior.FirstName = warriorDto.FirstName;
                if (warriorDto.NickName != warrior.NickName && warriorDto.NickName != null)
                    warrior.NickName = warriorDto.NickName;

                await _repositoryManager.warriorRepository.Update(warrior);
                await _repositoryManager.unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task DeleteAsync(int id)
        {
            var warrior = await _repositoryManager.warriorRepository.GetByIdAsync(id);
            if (warrior != null)
            {
                await _repositoryManager.warriorRepository.Remove(warrior);
                await _repositoryManager.unitOfWork.SaveChangesAsync();
            }
        }
    }
}
