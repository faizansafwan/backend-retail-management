using Microsoft.EntityFrameworkCore;
using retail_management.Models.Entities;

namespace retail_management.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Shop> Shops { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceProduct> InvoiceProducts { get; set; }
        public DbSet<InvoiceStock> InvoiceStocks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Shop)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Shop>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Set Id column to start from 100
            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .UseIdentityColumn(100, 1);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductName)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Shop)
                .WithMany(s => s.Customers)
                .HasForeignKey(c => c.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.CustomerName)
                .IsUnique();

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Stocks)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Stock>()
                .HasOne(e => e.Shop)
                .WithMany(e => e.Stocks)
                .HasForeignKey(e => e.ShopId)
                .OnDelete(DeleteBehavior.Restrict);

            // Invoice for Customer
            modelBuilder.Entity<Invoice>()
                .HasOne(e => e.Customer)
                .WithMany(e => e.Invoices)
                .HasForeignKey(e => e.CustomerId)
                .IsRequired();

            modelBuilder.Entity<InvoiceProduct>()
                .HasKey(e => new { e.InvoiceId, e.ProductId });

            modelBuilder.Entity<InvoiceProduct>()
                .HasOne(e => e.Invoice)
                .WithMany(e => e.InvoiceProducts)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InvoiceProduct>()
                .HasOne(e => e.Product)
                .WithMany(e => e.InvoiceProducts)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InvoiceStock>()
                .HasKey(e => new { e.InvoiceId, e.StockId });

            modelBuilder.Entity<InvoiceStock>()
                .HasOne(e => e.Invoice)
                .WithMany(e => e.InvoiceStocks)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InvoiceStock>()
                .HasOne(e => e.Stock)
                .WithMany(e => e.InvoiceStocks)
                .HasForeignKey(e => e.StockId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
