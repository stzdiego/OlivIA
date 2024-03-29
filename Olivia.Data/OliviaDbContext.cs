using Microsoft.EntityFrameworkCore;
using Olivia.Shared.Entities;

namespace Olivia.Data;

public class OliviaDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }

    public OliviaDbContext(DbContextOptions<OliviaDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}