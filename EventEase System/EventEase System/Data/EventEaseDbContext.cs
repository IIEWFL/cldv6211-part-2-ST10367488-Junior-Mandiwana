using System;
using System.Collections.Generic;
using EventEase_System.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEase_System.Data;

public partial class EventEaseDbContext : DbContext
{
   

    public EventEaseDbContext(DbContextOptions<EventEaseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

    public virtual DbSet<VwBookingDetail> VwBookingDetails { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951AEDE29E6E70");

            entity.ToTable("Booking");

            entity.HasOne(d => d.Event).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK__Booking__EventId__3D5E1FD2");

            entity.HasOne(d => d.Venue).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.VenueId)
                .HasConstraintName("FK__Booking__VenueId__3E52440B");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Event__7944C81025E68BC1");

            entity.ToTable("Event");

            entity.HasIndex(e => e.EventName, "UQ__Event__59D2B72693085D70").IsUnique();

            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.EventName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.VenueId).HasName("PK__Venue__3C57E5F204768CFA");

            entity.ToTable("Venue");

            entity.HasIndex(e => e.VenueName, "UQ__Venue__A40F8D1287B6D71B").IsUnique();

            entity.Property(e => e.ImageUrl).IsUnicode(false);
            entity.Property(e => e.Location)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VenueName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwBookingDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_BookingDetails");

            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.EventName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Location)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VenueName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
