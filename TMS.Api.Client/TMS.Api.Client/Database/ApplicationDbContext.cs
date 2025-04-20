using Microsoft.EntityFrameworkCore;

namespace TMS.Api.Client.Database;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder) { }
}
