using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Text;
using TicketsSystem.Core.Validations;
using TicketsSystem.Data.DTOs;
using TicketsSystem.Data.Repositories;
using TicketsSystem_Data;
using TicketsSystem_Data.Repositories;

namespace TicketsSystem.Core.Services
{
    public interface ITicketsService
    {
        Task<Result> AcceptTickets(string ticketIdStr);
        Task<Result> AssingTicketAsync(string userIdStr, string ticketIdSrt);
        Task<Result> ChangeTicketPriorityAsync(string ticketIdStr, int priorityId);
        Task<Result> ChangeTicketStatusAsync(string ticketIdStr, int statusId);
        Task<Result> CreateATicketAsync(TicketsDTO ticketsDTO);
        Task<Result<IEnumerable<TicketsDTO>>> GetAllTicketsAsync();
        Task<Result<IEnumerable<TicketsDTO>>> GetCurrentUserTicketsAsync();
        Task<Result<IEnumerable<TicketsDTO>>> GetTicketsByUserIdAsync(string userIdStr);
    }

    public class TicketsService : ITicketsService
    {
        private readonly ITicketsRepository _ticketsRepository;
        private readonly TicketsDTOValidator _ticketsDTOValidator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IGetUserRole _getUseRole;
        public TicketsService(ITicketsRepository ticketsRepository, 
            TicketsDTOValidator validationRules, 
            ICurrentUserService currentUserService,
            IGetUserRole getUserRole)
        {
            _ticketsRepository = ticketsRepository;
            _ticketsDTOValidator = validationRules;
            _currentUserService = currentUserService;
            _getUseRole = getUserRole;
        }

        public async Task<Result<IEnumerable<TicketsDTO>>> GetAllTicketsAsync()
        {
            var tickets = await _ticketsRepository.GetAllTickets();

            IEnumerable<TicketsDTO> ticketsDTOs = tickets.Select(t => new TicketsDTO
            {
                Title = t.Title,
                Description = t.Description,
                StatusId = t.StatusId,
                StatusName = t.Status.Name ?? "No status",
                PriorityId = t.PriorityId,
                PriorityName = t.Priority.Name ?? "No priority",
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                ClosedAt = t.ClosedAt
            });

            return Result.Ok(ticketsDTOs);
        }

        public async Task<Result<IEnumerable<TicketsDTO>>> GetCurrentUserTicketsAsync()
        {
            Guid currentUserId = _currentUserService.GetCurrentUserId();

            var tickets = await _ticketsRepository.GetCurrentUserTickets(currentUserId);

            IEnumerable<TicketsDTO> ticketsDTOs = tickets.Select(t => new TicketsDTO
            {
                Title = t.Title,
                Description = t.Description,
                StatusId = t.StatusId,
                StatusName = t.Status.Name ?? "No status",
                PriorityId = t.PriorityId,
                PriorityName = t.Priority.Name ?? "No priority",
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                ClosedAt = t.ClosedAt
            });

            return Result.Ok(ticketsDTOs);
        }

        public async Task<Result<IEnumerable<TicketsDTO>>> GetTicketsByUserIdAsync(string userIdStr)
        {
            Guid userId = Guid.Parse(userIdStr);

            var tickets = await _ticketsRepository.GetCurrentUserTickets(userId);

            IEnumerable<TicketsDTO> ticketsDTOs = tickets.Select(t => new TicketsDTO
            {
                Title = t.Title,
                Description = t.Description,
                StatusId = t.StatusId,
                StatusName = t.Status.Name ?? "No status",
                PriorityId = t.PriorityId,
                PriorityName = t.Priority.Name ?? "No priority",
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                ClosedAt = t.ClosedAt
            });

            return Result.Ok(ticketsDTOs);
        }

        public async Task<Result> CreateATicketAsync(TicketsDTO ticketsDTO)
        {
            if (ticketsDTO == null) 
                throw new ArgumentNullException(nameof(ticketsDTO));

            var validationResults = await _ticketsDTOValidator.ValidateAsync(ticketsDTO);
            if (!validationResults.IsValid)
            {
                var errorMessages = validationResults.Errors.Select(e => e.ErrorMessage);
                return Result.Fail(errorMessages);
            }

            Guid userId = _currentUserService.GetCurrentUserId();

            var newTicket = new Ticket
            {
                Title = ticketsDTO.Title,
                Description = ticketsDTO.Description,
                StatusId = ticketsDTO.StatusId,
                PriorityId = ticketsDTO.PriorityId,
                CreatedByUserId = userId
            };

            await _ticketsRepository.CreateTicket(newTicket);

            return Result.Ok();
        }

        public async Task<Result> ChangeTicketStatusAsync(string ticketIdStr, int statusId)
        {
            if (string.IsNullOrWhiteSpace(ticketIdStr))
                return Result.Fail("Ticket is bad formated");

            if (statusId <= 0 || statusId > 5)
                return Result.Fail("The status code it does not exist");

            Guid ticketId = Guid.Parse(ticketIdStr);
            var userId = _currentUserService.GetCurrentUserId();
            var ticket = await _ticketsRepository.GetTicketById(ticketId);
            var userRole = _currentUserService.GetCurrentUserRole();

            if (ticket == null)
                return Result.Fail("The ticket it does not exist");

            if (userRole == "Agent")
            {
                if (userId != ticket.AssignedToUserId)
                    return Result.Fail("Your not the agent assigned to this ticket");
            }

            ticket.StatusId = statusId;

            await _ticketsRepository.UpdateTicketInfo(ticket);

            return Result.Ok();
        }

        public async Task<Result> ChangeTicketPriorityAsync(string ticketIdStr, int priorityId)
        {
            if (string.IsNullOrWhiteSpace(ticketIdStr))
                return Result.Fail("Ticket is bad formated");

            if (priorityId <= 0 || priorityId > 4)
                return Result.Fail("The priority code is not exist");

            Guid ticketId = Guid.Parse(ticketIdStr);
            var userId = _currentUserService.GetCurrentUserId();
            var ticket = await _ticketsRepository.GetTicketById(ticketId);

            if (ticket == null)
                return Result.Fail("The ticket does not exist");

            var userRole = _currentUserService.GetCurrentUserRole();
            if (userRole == "User")
            {
                if (userId != ticket.CreatedByUserId)
                    return Result.Fail("UnAuthorized");
            }
            else if (userRole == "Agent")
            {
                if (userId != ticket.AssignedToUserId)
                    return Result.Fail("Your not the agent assigned to this ticket");
            }

            ticket.PriorityId = priorityId;

            await _ticketsRepository.UpdateTicketInfo(ticket);

            return Result.Ok();
        }

        public async Task<Result> AssingTicketAsync(string userIdStr, string ticketIdSrt)
        {
            if (string.IsNullOrWhiteSpace(ticketIdSrt) || ticketIdSrt == null)
                return Result.Fail("The id is not valid");

            if (string.IsNullOrWhiteSpace(userIdStr) || userIdStr == null)
                return Result.Fail("The userId is no valid");

            Guid userId = Guid.Parse(userIdStr);
            Guid ticketId = Guid.Parse(ticketIdSrt);

            if (await _getUseRole.UserIsAgent(userId) == false)
                return Result.Fail("The user is not Agent");

            var ticket = await _ticketsRepository.GetTicketById(ticketId);

            if (ticket == null)
                return Result.Fail("The ticket does not exits");

            ticket.AssignedToUserId = userId;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketsRepository.AssingTicket(ticket);

            return Result.Ok();
        }

        public async Task<Result> AcceptTickets(string ticketIdStr)
        {
            if (string.IsNullOrWhiteSpace(ticketIdStr) || ticketIdStr == null)
                return Result.Fail("The id is not valid");

            Guid userId = _currentUserService.GetCurrentUserId();
            Guid ticketId = Guid.Parse(ticketIdStr);

            if (await _getUseRole.UserIsAgent(userId) == false)
                return Result.Fail("The user is not Agent");

            var ticket = await _ticketsRepository.GetTicketById(ticketId);

            if (ticket == null)
                return Result.Fail("The ticket does not exits");

            ticket.AssignedToUserId = userId;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketsRepository.AssingTicket(ticket);

            return Result.Ok();
        }

    }
}
