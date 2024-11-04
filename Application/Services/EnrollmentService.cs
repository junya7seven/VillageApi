using Application.DTOs;
using Application.Interfaces;
using Entities.Interfaces;
using Entities.Models;
using Mapster;
using System;
using System.Collections;
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

        public async Task<IEnumerable<EnrollmentDTO>> GetAllAsync()
        {
            var enrollments = await _repositoryManager.warriorRepository.GetAllAsync();
            var enrollmentDto = enrollments.Adapt<IEnumerable<EnrollmentDTO>>();
            return enrollmentDto;
        }

        public async Task<EnrollmentDTO> GetByIdAsync(int id)
        {
            var enrollment = await _repositoryManager.enrollmentRepository.GetByIdAsync(id);
            if (enrollment == null) { }
            /// exception
            var enrollmentDto = enrollment.Adapt<EnrollmentDTO>();
            return enrollmentDto;
        }

        public async Task<EnrollmentDTO> CreateAsync(EnrollmentDTO enrollmentDto)
        {
            var enrollment = enrollmentDto.Adapt<Enrollment>();
            await _repositoryManager.enrollmentRepository.Insert(enrollment);
            return enrollment.Adapt(enrollmentDto);
        }

        public async Task UpdateAsync(int id, EnrollmentDTO enrollmentDto)
        {
            var enrollment = await _repositoryManager.enrollmentRepository.GetByIdAsync(id);
            if (enrollment is null) { }
            /// exception
            enrollment.WarriorId = enrollmentDto.WarriorId;
            enrollment.QuestId = enrollmentDto.QuestId;
            enrollment.Level = enrollment.Level;
            await _repositoryManager.enrollmentRepository.Update(enrollment);
            await _repositoryManager.unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var enrollment = await _repositoryManager.enrollmentRepository.GetByIdAsync(id);
            if (enrollment is null) { }
            /// exception
            _repositoryManager.enrollmentRepository.Remove(enrollment);
            await _repositoryManager.unitOfWork.SaveChangesAsync();
        }
    }
}
