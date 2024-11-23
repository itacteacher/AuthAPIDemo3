using AuthAPIDemo3.Data;
using AuthAPIDemo3.Entities;
using AuthAPIDemo3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPIDemo3.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TicketsController (AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTickets ()
    {
        var tickets = _context.Tickets.ToListAsync();

        return Ok(tickets);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddTicket ([FromBody] TicketModel ticket)
    {
        var entity = new Ticket
        {
            EventTitle = ticket.EventTitle,
            EventDate = ticket.EventDate,
            Price = ticket.Price
        };

        _context.Tickets.Add(entity);
        await _context.SaveChangesAsync();

        return Ok(entity.Id);
    }
}
