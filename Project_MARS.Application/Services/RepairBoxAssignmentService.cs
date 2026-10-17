using Microsoft.Extensions.Logging;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public class RepairBoxAssignmentService(
        IRepairBoxAssignmentRepository assignmentRepository, 
        IRepairBoxRepository repairBoxRepository,
        IAutoServiceRepository autoServiceRepository,
        ILogger<RepairBoxAssignmentService> logger) : IRepairBoxAssignmentService
    {
        public async Task<Result<List<RepairBoxAssignment>>> GetByServiceAsync(int serviceId)
        {
            var assignments = await assignmentRepository.GetByServiceAsync(serviceId);
            return Result<List<RepairBoxAssignment>>.Success(assignments);
        }

        public async Task<Result> UnassignWorkerAsync(string workerUserId, string currentBossUserId)
        {
            if (string.IsNullOrWhiteSpace(workerUserId))
            {
                return Result.Failure("Worker User ID cannot be empty.", ErrorType.BadRequest);
            }

           
            var allAssignments = await assignmentRepository.GetAsync();
            var activeAssignment = allAssignments
                .FirstOrDefault(a => a.WorkerUserId == workerUserId && a.IsActive);

            if (activeAssignment == null)
            {
                return Result.Failure("Active assignment not found for this worker.", ErrorType.NotFound);
            }

            
            var targetBox = await repairBoxRepository.GetAsync(activeAssignment.RepairBoxId);
            if (targetBox != null)
            {
                var autoService = await autoServiceRepository.GetAsync(targetBox.AutoServiceId);
                if (autoService == null || autoService.AutoServiceBoss?.UserId != currentBossUserId)
                {
                    return Result.Failure("You can only unassign workers from boxes within your own auto service.", ErrorType.Unauthorized);
                }
            }

          
            activeAssignment.IsActive = false;
            activeAssignment.UnassignedAt = DateTime.UtcNow;
            await assignmentRepository.UpdateAsync(activeAssignment);

            logger.LogInformation("Worker {WorkerId} unassigned from box by Boss {BossId}", workerUserId, currentBossUserId);
            return Result.Success();
        }

        public async Task<Result> UpdateAsync(string workerUserId, int repairBoxId, string currentBossUserId)
        {
            if (string.IsNullOrWhiteSpace(workerUserId))
            {
                return Result.Failure("Worker User ID cannot be empty.", ErrorType.BadRequest);
            }

          
            var targetBox = await repairBoxRepository.GetAsync(repairBoxId);
            if (targetBox == null)
            {
                return Result.Failure("Target repair box not found.", ErrorType.NotFound);
            }


            var autoService = await autoServiceRepository.GetAsync(targetBox.AutoServiceId);
            if (autoService == null || autoService.AutoServiceBoss?.UserId != currentBossUserId)
            {
                return Result.Failure("You can only assign workers to boxes within your own auto service.", ErrorType.Unauthorized);
            }

            
            var allAssignments = await assignmentRepository.GetAsync();
            var activeAssignment = allAssignments
                .FirstOrDefault(a => a.WorkerUserId == workerUserId && a.IsActive);

            if (activeAssignment != null)
            {
                activeAssignment.IsActive = false;
                activeAssignment.UnassignedAt = DateTime.UtcNow;
                await assignmentRepository.UpdateAsync(activeAssignment);
            }


            var newAssignment = new RepairBoxAssignment
            {
                RepairBoxId = repairBoxId,
                WorkerUserId = workerUserId,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            await assignmentRepository.AddAsync(newAssignment);
            logger.LogInformation("Worker {WorkerId} successfully moved to Repair Box ID {BoxId} by Boss {BossId}", workerUserId, repairBoxId, currentBossUserId);

            return Result.Success();
        }
    }
}