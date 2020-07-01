using Microsoft.EntityFrameworkCore;

namespace DocManagementAPI.Models
{
    public class DocumentContext: DbContext
    {
        public DocumentContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }
    }
}
