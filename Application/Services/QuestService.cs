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
            var questsDto = quests.Adapt<IEnumerable<QuestDTO>>();
            return questsDto;
        }

        public async Task<QuestDTO> GetByIdAsync(int id)
        {
            var quest = await _repositoryManager.questRepository.GetByIdAsync(id);
            if (quest == null) { }
                /// exception
            var questDto = quest.Adapt<QuestDTO>();
            return questDto;
        }

        public async Task<QuestDTO> CreateAsync(QuestDTO questDto)
        {
            var quest = questDto.Adapt<Quest>();
            quest.Adapt<QuestDTO>();
            await _repositoryManager.questRepository.Insert(quest);
            return quest.Adapt(questDto);
        }

        public async Task UpdateAsync(int id, QuestDTO questDto)
        {
            var quest = await _repositoryManager.questRepository.GetByIdAsync(id);
            if(quest is null) { }
                /// exception
            quest.QuestId = questDto.QuestId;
            quest.Description = questDto.Description;
            quest.Reward = questDto.Reward;
            await _repositoryManager.unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var quest = await _repositoryManager.questRepository.GetByIdAsync(id);
            if (quest is null) { }
            /// exception
            await _repositoryManager.questRepository.Remove(quest);
            await _repositoryManager.unitOfWork.SaveChangesAsync();
        }
    }
