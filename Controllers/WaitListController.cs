using Linnked.Core.Domain.Entities;
using Linnked.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[Route("api/[controller]")]
[ApiController]
public class WaitListController : ControllerBase
{
    private readonly AppDbContext _context;

    public WaitListController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddToWaitList([FromBody] WaitListDTO waitList)
    {
        if (waitList == null || string.IsNullOrWhiteSpace(waitList.FirstName) || string.IsNullOrWhiteSpace(waitList.Email))
        {
            return BadRequest("Invalid input data.");
        }
        bool exists = await _context.WaitLists
                                    .AnyAsync(w => w.Email == waitList.Email);

        if (exists)
        {
            return Conflict(new { message = "You are already on the waitlist!" });
        }
        WaitList waitList1 = new WaitList()
        {
            Email = waitList.Email,
            FirstName = waitList.FirstName,
            DateCreated = DateTime.UtcNow,
        };
        _context.WaitLists.Add(waitList1);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Successfully added to the waitlist!" });
    }

}
