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
    internal sealed class EnrollmentService : IEnrollmentService
    {
        private readonly IRepositoryManager _repositoryManager;
        public EnrollmentService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            var enrollments = await _repositoryManager.enrollmentRepository.GetAllAsync();
            if (!enrollments.Any())
                return null;
            return enrollments;
        }

        public async Task<Enrollment> GetByIdAsync(int id)
        {
            var enrollment = await _repositoryManager.enrollmentRepository.GetByIdAsync(id);
            if (enrollment == null) { }
            /// exception
            return enrollment;
        }

        public async Task<EnrollmentDTO> CreateAsync(EnrollmentDTO enrollmentDto)
        {
            var enrollment = enrollmentDto.Adapt<Enrollment>();
            var warriorId = await _repositoryManager.warriorRepository.GetByIdAsync(enrollmentDto.WarriorId);
            var questId = await _repositoryManager.questRepository.GetByIdAsync(enrollmentDto.QuestId);
            if(warriorId == null || questId == null)
            {
                await _repositoryManager.enrollmentRepository.Insert(enrollment);
                await _repositoryManager.unitOfWork.SaveChangesAsync();
                return enrollment.Adapt(enrollmentDto);
            }
            return null;
        }

        public async Task UpdateAsync(int id, EnrollmentDTO enrollmentDto)
        {
            var enrollment = await _repositoryManager.enrollmentRepository.GetByIdAsync(id);
            if (enrollment is null) { }
            /// exception
            var warriorId = await _repositoryManager.warriorRepository.GetByIdAsync(enrollmentDto.WarriorId);
            var questId = await _repositoryManager.questRepository.GetByIdAsync(enrollmentDto.QuestId);
            if (warriorId == null || questId == null)
            {
                enrollment.WarriorId = enrollmentDto.WarriorId;
                enrollment.QuestId = enrollmentDto.QuestId;
                enrollment.Level = enrollment.Level;
                await _repositoryManager.unitOfWork.SaveChangesAsync();
            }
        }
        public async Task DeleteAsync(int id)
        {
            var enrollment = await _repositoryManager.enrollmentRepository.GetByIdAsync(id);
            if (enrollment is null) { }
            /// exception
            await _repositoryManager.enrollmentRepository.Remove(enrollment);
            await _repositoryManager.unitOfWork.SaveChangesAsync();
        }
    }
}
