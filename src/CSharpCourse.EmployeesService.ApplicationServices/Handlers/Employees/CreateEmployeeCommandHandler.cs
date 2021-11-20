using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using AutoMapper;
using Confluent.Kafka;
using CSharpCourse.Core.Lib.Enums;
using CSharpCourse.Core.Lib.Events;
using CSharpCourse.EmployeesService.ApplicationServices.MessageBroker;
using CSharpCourse.EmployeesService.Domain.Models.Entities;
using CSharpCourse.EmployeesService.ApplicationServices.Models.Commands;
using CSharpCourse.EmployeesService.Domain.Contracts.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CSharpCourse.EmployeesService.ApplicationServices.Handlers.Employees
{
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, long>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly IProducerBuilderWrapper _producerBuilderWrapper;
        private readonly ILogger<CreateEmployeeCommandHandler> _logger;

        public CreateEmployeeCommandHandler(IEmployeeRepository repository,
            IMapper mapper,
            IProducerBuilderWrapper producerBuilderWrapper,
            ILogger<CreateEmployeeCommandHandler> logger = null)
        {
            _employeeRepository = repository;
            _mapper = mapper;
            _producerBuilderWrapper = producerBuilderWrapper;
            _logger = logger ?? NullLogger<CreateEmployeeCommandHandler>.Instance;
        }

        public async Task<long> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            // Подготовка модели для создания нового сотрудника
            var dto = _mapper.Map<Employee>(request);
            dto.HiringDate = DateTime.UtcNow;
            dto.FiredDate = null;

            // Создаем сотрудника
            var employeeId = await _employeeRepository.CreateAsync(dto, cancellationToken);

            // Отправляем заявку, что сотрудник создан, и нужно выдать мерч
            await _producerBuilderWrapper.Producer.ProduceAsync(_producerBuilderWrapper.CreateNewEmployeeTopic,
                new Message<string, string>()
                {
                    Key = employeeId.ToString(),
                    Value = JsonSerializer.Serialize(new NotificationEvent()
                    {
                        EmployeeEmail = dto.Email,
                        EmployeeName = $"{dto.LastName} {dto.FirstName} {dto.MiddleName}",
                        EventType = EmployeeEventType.Hiring,
                        Payload = new MerchDeliveryEventPayload()
                        {
                            MerchType = MerchType.WelcomePack
                        }
                    })
                }, cancellationToken);

            return employeeId;
        }
    }
}
