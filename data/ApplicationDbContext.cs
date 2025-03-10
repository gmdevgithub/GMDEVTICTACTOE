// filepath: /c:/SpecterClearLLC/TicTacToeWeb/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace TicTacToeWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define your DbSets here
    }
}