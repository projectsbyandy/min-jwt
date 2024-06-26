using Keyboard.Common.Models;
using KeyboardApi.Repository.Vendor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace KeyboardApi.Routes;

public static class VendorApi
{
    public static void AddVendorRoutes(this IEndpointRouteBuilder app)
    {
        // Setup Vendor Api
        app.MapGet("/keyboard/vendors", async (VendorDbContext db) => await db.Vendors.ToListAsync());

        app.MapGet("/keyboard/vendor-with-auth", [Authorize] async (VendorDbContext db) => await db.Vendors.ToListAsync());

        app.MapGet("/keyboard/vendors/{id}", async (Guid id, VendorDbContext db) =>
            await db.Vendors.FindAsync(id)
                is { } vendor
                ? Results.Ok(vendor)
                : Results.NotFound());

        app.MapPost("/keyboard/vendor", async (Vendor vendor, VendorDbContext db) =>
        {
            db.Vendors.Add(vendor);

            try
            {
                await db.SaveChangesAsync();
        
                return Results.Created($"/keyboard/vendors/{vendor.Id}", vendor);
            }
            catch (ArgumentException ex)
            {
                return Results.Conflict(ex.Message);
            }
        });

        app.MapPut("/keyboard/vendors/{id}", async (Guid id, Vendor vendor, VendorDbContext db) =>
        {
            var vendorToUpdate = await db.Vendors.FindAsync(id);

            if (vendorToUpdate is null) 
                return Results.NotFound("Vendor not found");

            vendorToUpdate.Name = vendor.Name;
            
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}