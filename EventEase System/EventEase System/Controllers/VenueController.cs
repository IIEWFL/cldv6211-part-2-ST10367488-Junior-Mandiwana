using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase_System.Data;
using EventEase_System.Models;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Azure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace EventEase_System.Controllers
{
    [Authorize]
    public class VenueController : Controller
    {
        private readonly EventEaseDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly int _sasExpiryHours;

        public VenueController(EventEaseDbContext context, IConfiguration config)
        {
            _context = context;
            _containerName = config["AzureBlob:ContainerName"] ?? "venues";
            _sasExpiryHours = config.GetValue<int>("AzureBlob:SASTokenExpiryHours", 24);
            _blobServiceClient = new BlobServiceClient(config.GetConnectionString("AzureBlobStorage"));
        }

        // GET: Venue
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        // GET: Venue/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);

            return venue == null ? NotFound() : View(venue);
        }

        // GET: Venue/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venue/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("VenueId,VenueName,Location,Capacity")] Venue venue, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("ImageUrl", "Venue image is required");
                return View(venue);
            }

            try
            {
                venue.ImageUrl = await UploadImageAsync(imageFile);

                if (ModelState.IsValid)
                {
                    _context.Add(venue);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error uploading image: {ex.Message}");
            }

            return View(venue);
        }

        // GET: Venue/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FindAsync(id);
            return venue == null ? NotFound() : View(venue);
        }

        // POST: Venue/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue, IFormFile imageFile)
        {
            if (id != venue.VenueId) return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                try
                {
                    venue.ImageUrl = await UploadImageAsync(imageFile);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ImageUrl", $"Error uploading image: {ex.Message}");
                    return View(venue);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId)) return NotFound();
                    throw;
                }
            }
            return View(venue);
        }

        // GET: Venue/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);

            return venue == null ? NotFound() : View(venue);
        }

        // POST: Venue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }

        private async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            var blobName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = imageFile.ContentType });
            }

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(_sasExpiryHours),
                Protocol = SasProtocol.Https
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var storageAccountName = _blobServiceClient.AccountName;
            var storageAccountKey = Environment.GetEnvironmentVariable("AZURE_STORAGE_KEY")
                ?? throw new InvalidOperationException("Azure Storage Key not configured");

            var sasToken = sasBuilder.ToSasQueryParameters(
                new StorageSharedKeyCredential(storageAccountName, storageAccountKey)).ToString();

            return $"{blobClient.Uri}?{sasToken}";
        }
    }
}