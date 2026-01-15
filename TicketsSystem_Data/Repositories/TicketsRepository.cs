using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TicketsSystem_Data;

namespace TicketsSystem.Data.Repositories
{
    public interface ITicketsRepository
    {
        Task AcceptTickets(Ticket ticketAccepted);
        Task AssingTicket(Ticket ticketWithAssingUserId);
        Task CreateTicket(Ticket newTicket);
        Task<IEnumerable<Ticket>> GetAllTickets();
        Task<IEnumerable<Ticket>> GetCurrentUserTickets(Guid currentUserId, string userRole);
        Task<Ticket?> GetTicketById(Guid ticketId);
        Task<IEnumerable<Ticket>> GetTicketsByUserId(Guid userId);
        Task<IEnumerable<Ticket?>> SearchTickets(string query, int? statusId, int? priorityId);
        Task UpdateTicketInfo(Ticket ticket);
    }

    public class TicketsRepository : ITicketsRepository
    {
        private readonly SystemTicketsContext _context;
        public TicketsRepository(SystemTicketsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ticket>> GetAllTickets()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.AssignedToUser)
                .ToListAsync();
            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetCurrentUserTickets(Guid currentUserId, string userRole)
        {
            var queryable = _context.Tickets.Where(t => t.CreatedByUserId == currentUserId || currentUserId == t.AssignedToUserId);

            if (userRole != "Agent")
                queryable = queryable.Where(t => t.CreatedByUserId == currentUserId);

            return await queryable
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByUserId(Guid userId)
        {
            return await _context.Tickets
                .Where(t => t.CreatedByUserId == userId)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .ToListAsync();
        }

        public async Task<Ticket?> GetTicketById(Guid ticketId)
        {
            return await _context.Tickets
                .FirstOrDefaultAsync(t =>  t.TicketId == ticketId);
        }

        public Task CreateTicket(Ticket newTicket)
        {
            _context.Tickets.Add(newTicket);
            return _context.SaveChangesAsync();
        }

        public Task UpdateTicketInfo(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            return _context.SaveChangesAsync();
        }

        public Task AssingTicket(Ticket ticketWithAssingUserId)
        {
            _context.Tickets.Update(ticketWithAssingUserId);
            return _context.SaveChangesAsync();
        }

        public Task AcceptTickets(Ticket ticketAccepted)
        {
            _context.Tickets.Update(ticketAccepted);
            return _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Ticket?>> SearchTickets(string query, int? statusId, int? priorityId)
        {
            var queryable = _context.Tickets.Where(t => t.Title.ToLower().Contains(query));

            if (statusId.HasValue)
                queryable = queryable.Where(t => t.StatusId == statusId.Value);

            if (priorityId.HasValue)
                queryable = queryable.Where(t => t.PriorityId == priorityId.Value);

            return await queryable
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .ToListAsync();
        }

    }
}
