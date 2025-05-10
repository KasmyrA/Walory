using Microsoft.EntityFrameworkCore;

namespace Infrastracture
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

    }
}
