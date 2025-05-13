using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase_System.Data;
using EventEase_System.Models;
using System.Threading.Tasks;
using System.Linq;

public class BookingDetailsController : Controller
{
    private readonly EventEaseDbContext _context;

    public BookingDetailsController(EventEaseDbContext context)
    {
        _context = context;
    }

    // GET: BookingDetails
    public async Task<IActionResult> BookingDetails(string searchTerm)
    {
        var query = _context.VwBookingDetails.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(b =>
                (b.EventName != null && b.EventName.Contains(searchTerm)) ||
                b.BookingId.ToString().Contains(searchTerm));
        }


        var bookingList = await query.ToListAsync();
        return View("Index", bookingList); // Explicitly load Index.cshtml

    }


    public IActionResult Details()
    {
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }





    public IActionResult Edit()
    {
        return View();
    }


    public IActionResult Delete()
    {
        return View();
    }
}
