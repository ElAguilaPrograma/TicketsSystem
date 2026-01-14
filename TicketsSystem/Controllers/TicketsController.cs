using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using TicketsSystem.Core.Errors;
using TicketsSystem.Core.Services;
using TicketsSystem.Data.DTOs;
using TicketsSystem.Data.DTOs.TicketsDTO;

namespace TicketsSystem.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ApiBaseController
    {
        private readonly ITicketsService _ticketsService;
        public TicketsController(ITicketsService ticketsService)
        {
            _ticketsService = ticketsService;
        }

        [HttpGet("getalltickets")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTickets()
            => ProcessResult(await _ticketsService.GetAllTicketsAsync());

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
        public async Task<IActionResult> CreateTicket([FromBody] TicketsCreateDto ticketsCreateDto)
        {
            var result = await _ticketsService.CreateATicketAsync(ticketsCreateDto);

            if (result.IsSuccess) return Ok();

            if (result.HasError<NotFoundError>())
                return NotFound(result.Errors.First().Message);

            return BadRequest(result.Errors.Select(e => e.Message));
        }

        [HttpPost("updateticketinfo/{ticketId}")]
        [Authorize(Roles = "Admin, Agent")]
        public async Task<IActionResult> UpdateTicket([FromBody] TicketsUpdateDto tickets, string ticketId)
        {
            var result = await _ticketsService.UpdateATicketInfoAsync(tickets, ticketId);

            if (result.IsFailed)
                return BadRequest(new { errors = result.Errors.Select(e => e.Message) });

            return Ok();
        }

        [HttpPost("updateticketpriority")]
        public async Task<IActionResult> UpdateTicketPriorityLevel([FromBody] TicketsUpdateDto ticketsUpdateDto, string ticketId)
        {
            var result = await _ticketsService.UpdateATicketInfoUserRoleAsync(ticketsUpdateDto, ticketId);

            if (result.IsFailed) 
                return BadRequest(new { errors = result.Errors?.Select(e => e.Message) });

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

    }
}
