using Microsoft.EntityFrameworkCore;
using HostelManage.Models;
using System.Collections.Generic;
using HostelManage.Models;

namespace HostelManage.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet properties for each model
        public DbSet<User> User { get; set; }
        public DbSet<Hostel> Hostel { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<HostelRoom> HostelRoom { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<HostelDescription> HostelDescription { get; set; }

        public DbSet<Feedback> Feedback { get; set; }
    }
}
