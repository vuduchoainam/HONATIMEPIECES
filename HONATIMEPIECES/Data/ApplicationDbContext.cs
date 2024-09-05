using HONATIMEPIECES.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HONATIMEPIECES.Data
{
    public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyValue> PropertyValues { get; set; }
        public DbSet<PropertyProduct> PropertyProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Cascade delete for PropertyProduct when Product is deleted
            modelBuilder.Entity<PropertyProduct>()
                .HasOne(pp => pp.Product)
                .WithMany(p => p.PropertyProducts)
                .HasForeignKey(pp => pp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cascade delete for UploadImage when Product is deleted
            modelBuilder.Entity<UploadImage>()
                .HasOne(ui => ui.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(ui => ui.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}