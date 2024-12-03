using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ChirpDBContextFactory : IDesignTimeDbContextFactory<ChirpDBContext>
{
    public ChirpDBContext CreateDbContext(string[] args)
    {
        var optionsbuilder = new DbContextOptionsBuilder<ChirpDBContext>();

        var connString = "Data Source=../Chirp.Web/Chirp.db";
        optionsbuilder.UseSqlite(connString);

        return new ChirpDBContext(optionsbuilder.Options);
    }
}