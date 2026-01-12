using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketsSystem.Core.Services;
using TicketsSystem.Data.DTOs;

namespace TicketsSystem.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketsService _ticketsService;
        public TicketsController(ITicketsService ticketsService)
        {
            _ticketsService = ticketsService;
        }

        [HttpGet("getalltickets")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTickets()
        {
            var result = await _ticketsService.GetAllTicketsAsync();

            if (result.IsFailed)
                return BadRequest(new { errors = result.Errors.Select(e => e.Message) });

            return Ok(result.Value);
        }

        [HttpGet("getcurrentusertickets")]
        public async Task<IActionResult> GetCurrentUserTickets()
        {
            var result = await _ticketsService.GetCurrentUserTicketsAsync();

            if (result.IsFailed)
                return BadRequest(new { errors = result.Errors.Select(e => e.Message) });

            return Ok(result.Value);
        }

        [HttpGet("getticketsbyuserid/{userId}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> GetTicketsByUserId(string userId)
        {
            var result = await _ticketsService.GetTicketsByUserIdAsync(userId);

            if (result.IsFailed)
                return BadRequest(new { errors = result.Errors.Select(e => e.Message) });

            return Ok(result.Value);
        }

        [HttpPost("createticket")]
        public async Task<IActionResult> CreateTicket([FromBody] TicketsDTO ticketsDTO)
        {
            var result = await _ticketsService.CreateATicketAsync(ticketsDTO);

            if (result.IsFailed)
                return BadRequest(new { errors = result.Errors.Select(e => e.Message) });

            return Ok();
        }

        [HttpPost("assingtickets/{ticketId}/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssingTickets(string userId, string ticketId)
        {
            var result = await _ticketsService.AssingTicketAsync(userId, ticketId);

            if (result.IsFailed)
                return BadRequest(result.Errors.Select(e => e.Message));

            return Ok();
        }

        [HttpPost("accepttickets/{ticketId}")]
        [Authorize(Roles = "Admin, Agent")]
        public async Task<IActionResult> AcceptTicket(string ticketId)
        {
            var result = await _ticketsService.AcceptTickets(ticketId);

            if (result.IsFailed)
                return BadRequest(result.Errors.First().Message);

            return Ok();
        }

        [HttpPost("changeticketstatus/{ticketId}/{statusId}")]
        [Authorize(Roles = "Admin, Agent")]
        public async Task<IActionResult> ChangeTicketStatus(string ticketId, int statusId)
        {
            var result = await _ticketsService.ChangeTicketStatusAsync(ticketId, statusId);

            if (result.IsFailed)
                return BadRequest(result.Errors.First().Message);

            return Ok();
        }

        [HttpPost("changeticketpriority/{ticketId}/{priorityId}")]
        public async Task<IActionResult> ChangeTicketPriority(string ticketId, int priorityId)
        {
            var result = await _ticketsService.ChangeTicketPriorityAsync(ticketId, priorityId);

            if (result.IsFailed)
                return BadRequest(result.Errors.First().Message);

            return Ok();
        }
    }
}
