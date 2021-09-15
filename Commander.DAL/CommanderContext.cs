using Commander.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Commander.DAL
{
    public class CommanderContext : DbContext
    {
        public CommanderContext(DbContextOptions<CommanderContext> options) : base(options)
        {
            
        }
        
        public DbSet<Command> Commands { get; set; }
    }
}