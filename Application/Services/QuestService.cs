using Application.DTOs;
using Application.Interfaces;
using Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Entities.Models;

namespace Application.Services
{
    internal sealed class QuestService : IQuestService
    {
        private readonly IRepositoryManager _repositoryManager;
        public QuestService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<IEnumerable<QuestDTO>> GetAllAsync()
        {

            var quests = await _repositoryManager.questRepository.GetAllAsync();
            if (!quests.Any())
                return null;
            var questsDto = quests.Adapt<IEnumerable<QuestDTO>>();
            return questsDto;

        }

        public async Task<QuestDTO> GetByIdAsync(int id)
        {

            var quest = await _repositoryManager.questRepository.GetByIdAsync(id);
            if (quest == null)
                return null;
            var questDto = quest.Adapt<QuestDTO>();
            return questDto;

        }

        public async Task<QuestDTO> CreateAsync(QuestDTO questDto)
        {
            try
            {
                var quest = questDto.Adapt<Quest>();
                var exists = await _repositoryManager.questRepository.GetByIdAsync(questDto.QuestId);
                if (exists != null)
                    return null;
                await _repositoryManager.questRepository.Insert(quest);
                await _repositoryManager.unitOfWork.SaveChangesAsync();
                return quest.Adapt(questDto);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdateAsync(int id, QuestDTO questDto)
        {
            try
            {
                var quest = await _repositoryManager.questRepository.GetByIdAsync(id);
                if (quest is null)
                if(questDto.Description != quest.Description && questDto.Description != null) 
                    quest.Description = questDto.Description;
                if (questDto.Reward != quest.Reward && questDto.Reward > 0)
                    quest.Reward = questDto.Reward;
                
                await _repositoryManager.questRepository.Update(quest);
                await _repositoryManager.unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task DeleteAsync(int id)
        {
            var quest = await _repositoryManager.questRepository.GetByIdAsync(id);
            if (quest != null)
            {
                await _repositoryManager.questRepository.Remove(quest);
                await _repositoryManager.unitOfWork.SaveChangesAsync();
            }
        }
    }
}
