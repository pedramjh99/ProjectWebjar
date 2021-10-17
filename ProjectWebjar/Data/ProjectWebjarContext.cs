using Microsoft.EntityFrameworkCore;
using ProjectWebjar.Mapping;
using ProjectWebjar.Models;

namespace ProjectWebjar.Data
{
    public class ProjectWebjarContext : DbContext
    {
        public ProjectWebjarContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<AttributeProduct> AttributeProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductMapping());
            modelBuilder.ApplyConfiguration(new CommentMapping());
            modelBuilder.Entity<AttributeProduct>().HasKey(p =>new 
            {
                p.AttributeId,p.ProductId
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
