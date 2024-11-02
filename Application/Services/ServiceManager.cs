using Application.Interfaces;
using Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IEnrollmentService> _lazyEnrollmentService;
        private readonly Lazy<IQuestService> _lazyQuestService;
        private readonly Lazy<IWarriorService> _lazyWarriorService;

        public ServiceManager(IRepositoryManager repositoryManager)
        {
            _lazyEnrollmentService = new Lazy<IEnrollmentService>(() => new EnrollmentService(repositoryManager));
            _lazyQuestService = new Lazy<IQuestService>(() => new QuestService(repositoryManager));
            _lazyWarriorService = new Lazy<IWarriorService>(() => new WarriorService(repositoryManager));
        }
    }
}
