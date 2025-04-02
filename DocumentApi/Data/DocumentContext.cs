using Microsoft.EntityFrameworkCore;
using DocumentApi.Models;

namespace DocumentApi.Data
{
    public class DocumentContext : DbContext
    {
        public DocumentContext(DbContextOptions<DocumentContext> options) : base(options) { }

        public DbSet<Document> Documents { get; set; }
    }
}