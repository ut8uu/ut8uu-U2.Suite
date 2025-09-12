using Microsoft.EntityFrameworkCore;

using U2.Logger.Backend.Models;

namespace U2.Logger.Backend.Data;

public class LoggerContext : DbContext
{
    public LoggerContext(DbContextOptions<LoggerContext> options) : base(options)
    {
    }

    public DbSet<QSO> QSOs { get; set; }
}
