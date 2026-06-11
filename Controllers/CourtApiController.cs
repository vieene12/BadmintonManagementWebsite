using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AquarSmartCourt.Models;

namespace AquarSmartCourt.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CourtApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CourtApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/CourtApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Court>>> GetCourts()
    {
        return await _context.Courts.ToListAsync();
    }

    // GET: api/CourtApi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Court>> GetCourt(int id)
    {
        var court = await _context.Courts.FindAsync(id);
        if (court == null)
        {
            return NotFound();
        }
        return court;
    }

    // PUT: api/CourtApi/5
    [Authorize(Roles = "3")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCourt(int id, Court court)
    {
        if (id != court.CourtId)
        {
            return BadRequest("ID mismatch");
        }

        var dbCourt = await _context.Courts.FindAsync(id);
        if (dbCourt == null)
        {
            return NotFound();
        }

        dbCourt.CourtName = court.CourtName;
        dbCourt.HourlyPrice = court.HourlyPrice;
        if (!string.IsNullOrEmpty(court.Status))
        {
            dbCourt.Status = court.Status;
        }

        _context.Entry(dbCourt).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CourtExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/CourtApi
    [Authorize(Roles = "3")]
    [HttpPost]
    public async Task<ActionResult<Court>> PostCourt(Court court)
    {
        if (string.IsNullOrEmpty(court.CourtCode))
        {
            var count = await _context.Courts.CountAsync();
            court.CourtCode = $"S{count + 1:D2}";
        }

        _context.Courts.Add(court);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCourt), new { id = court.CourtId }, court);
    }

    // DELETE: api/CourtApi/5
    [Authorize(Roles = "3")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourt(int id)
    {
        var court = await _context.Courts.FindAsync(id);
        if (court == null)
        {
            return NotFound();
        }

        _context.Courts.Remove(court);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/CourtApi/services
    [HttpGet("services")]
    public async Task<ActionResult<IEnumerable<ServiceItem>>> GetServices()
    {
        return await _context.ServiceItems.ToListAsync();
    }

    // PUT: api/CourtApi/services/{id}
    [Authorize(Roles = "3")]
    [HttpPut("services/{id}")]
    public async Task<IActionResult> PutService(int id, ServiceItem item)
    {
        if (id != item.ServiceItemId)
        {
            return BadRequest("ID mismatch");
        }

        var dbItem = await _context.ServiceItems.FindAsync(id);
        if (dbItem == null)
        {
            return NotFound();
        }

        dbItem.ItemName = item.ItemName;
        dbItem.UnitPrice = item.UnitPrice;
        if (!string.IsNullOrEmpty(item.Unit))
        {
            dbItem.Unit = item.Unit;
        }
        if (!string.IsNullOrEmpty(item.Category))
        {
            dbItem.Category = item.Category;
        }

        _context.Entry(dbItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/CourtApi/services
    [Authorize(Roles = "3")]
    [HttpPost("services")]
    public async Task<ActionResult<ServiceItem>> PostService(ServiceItem item)
    {
        _context.ServiceItems.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetServices), new { id = item.ServiceItemId }, item);
    }

    // DELETE: api/CourtApi/services/{id}
    [Authorize(Roles = "3")]
    [HttpDelete("services/{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var item = await _context.ServiceItems.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        _context.ServiceItems.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // POST: api/CourtApi/order
    [HttpPost("order")]
    public async Task<IActionResult> OrderService([FromBody] OrderRequest request)
    {
        var court = await _context.Courts.FindAsync(request.CourtId);
        if (court == null) return NotFound("Court not found");

        var item = await _context.ServiceItems.FindAsync(request.ServiceItemId);
        if (item == null) return NotFound("Service item not found");

        var existing = await _context.ServiceOrders
            .FirstOrDefaultAsync(so => so.CourtId == request.CourtId && so.ServiceItemId == request.ServiceItemId);

        if (existing != null)
        {
            existing.Quantity += request.Quantity;
            _context.Entry(existing).State = EntityState.Modified;
        }
        else
        {
            var order = new ServiceOrder
            {
                CourtId = request.CourtId,
                ServiceItemId = request.ServiceItemId,
                Quantity = request.Quantity,
                OrderTime = DateTime.Now
            };
            _context.ServiceOrders.Add(order);
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    // GET: api/CourtApi/orders/{courtId}
    [HttpGet("orders/{courtId}")]
    public async Task<IActionResult> GetCourtOrders(int courtId)
    {
        var orders = await _context.ServiceOrders
            .Where(so => so.CourtId == courtId)
            .Include(so => so.ServiceItem)
            .Select(so => new {
                so.ServiceOrderId,
                so.ServiceItemId,
                ItemName = so.ServiceItem != null ? so.ServiceItem.ItemName : "",
                Unit = so.ServiceItem != null ? so.ServiceItem.Unit : "",
                UnitPrice = so.ServiceItem != null ? (double)so.ServiceItem.UnitPrice : 0.0,
                so.Quantity,
                Total = so.ServiceItem != null ? (double)(so.ServiceItem.UnitPrice * so.Quantity) : 0.0
            })
            .ToListAsync();

        return Ok(orders);
    }

    // DELETE: api/CourtApi/orders/{courtId}
    [Authorize(Roles = "2,3")]
    [HttpDelete("orders/{courtId}")]
    public async Task<IActionResult> ClearCourtOrders(int courtId)
    {
        var orders = await _context.ServiceOrders.Where(so => so.CourtId == courtId).ToListAsync();
        _context.ServiceOrders.RemoveRange(orders);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // POST: api/CourtApi/book
    [HttpPost("book")]
    public async Task<IActionResult> BookCourt([FromBody] BookingRequest request)
    {
        var court = await _context.Courts.FindAsync(request.CourtId);
        if (court == null) return NotFound("Court not found");

        court.Status = "InUse"; // Transition to InUse to simulate instant active play
        _context.Entry(court).State = EntityState.Modified;

        if (request.Services != null)
        {
            foreach (var svc in request.Services)
            {
                if (svc.Quantity > 0)
                {
                    var order = new ServiceOrder
                    {
                        CourtId = request.CourtId,
                        ServiceItemId = svc.ServiceItemId,
                        Quantity = svc.Quantity,
                        OrderTime = DateTime.Now
                    };
                    _context.ServiceOrders.Add(order);
                }
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Booking successful", courtName = court.CourtName });
    }

    private bool CourtExists(int id)
    {
        return _context.Courts.Any(e => e.CourtId == id);
    }
}

public class OrderRequest
{
    public int CourtId { get; set; }
    public int ServiceItemId { get; set; }
    public int Quantity { get; set; }
}

public class BookingRequest
{
    public int CourtId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public List<ServiceOrderItemDto> Services { get; set; } = new();
}

public class ServiceOrderItemDto
{
    public int ServiceItemId { get; set; }
    public int Quantity { get; set; }
}
