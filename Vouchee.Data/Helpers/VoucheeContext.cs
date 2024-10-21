using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Reflection.Emit;
using System.Xml.Linq;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Helpers
{
    public partial class VoucheeContext : DbContext
    {
        public VoucheeContext() { }

        public VoucheeContext(DbContextOptions<VoucheeContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<VoucherType> VoucherTypes { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<VoucherCode> VoucherCodes { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfigurationRoot configuration = builder.Build();
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("PROD"));
                /*optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=12345;database=Vouchee1;TrustServerCertificate=True;");*/
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<OrderDetail>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Role>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Address>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Supplier>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<User>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Voucher>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<VoucherCode>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<VoucherType>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Promotion>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Category>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Brand>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Media>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Cart>().HasKey(c => new { c.BuyerId, c.VoucherId });
            modelBuilder.Entity<SubVoucher>().Property(x => x.Id).HasDefaultValueSql("NEWID()");

            // modelBuilder.Seed();

            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateOnly>()
                                .HaveConversion<DateOnlyConverter>()
                                .HaveColumnType("date");

            base.ConfigureConventions(configurationBuilder);
        }
    }
}
