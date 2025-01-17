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
        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<VoucherType> VoucherTypes { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<VoucherCode> VoucherCodes { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Modal> Modals { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<MoneyRequest> MoneyRequests { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }
        public virtual DbSet<WalletTransaction> WalletTransactions { get; set; }
        public virtual DbSet<PartnerTransaction> PartnerTransactions { get; set; }
        public virtual DbSet<Media> Medias { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<DeviceToken> DeviceTokens { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<RefundRequest> RefundRequests { get; set; }
        public virtual DbSet<Report> Reports { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
                IConfigurationRoot configuration = builder.Build();
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.UseLazyLoadingProxies();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("PROD"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
            modelBuilder.Entity<Cart>().HasKey(c => new { c.BuyerId, c.ModalId });
            modelBuilder.Entity<Modal>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Notification>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Wallet>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<WalletTransaction>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<PartnerTransaction>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Rating>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<DeviceToken>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<RefundRequest>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Report>().Property(x => x.Id).HasDefaultValueSql("NEWID()");

            // modelBuilder.Seed();
            //modelBuilder.Entity<User>()
            //                .HasOne(u => u.Supplier)
            //                .WithOne(s => s.User)
            //                .HasForeignKey<Supplier>(s => s.UserId);

            modelBuilder.Entity<OrderDetail>()
                            .HasKey(od => new { od.OrderId, od.ModalId });

            modelBuilder.Entity<Modal>()
                            .HasMany(p => p.VoucherCodes)
                            .WithOne(c => c.Modal)
                            .HasForeignKey(c => c.ModalId)
                            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Voucher>()
                            .HasMany(p => p.Medias)
                            .WithOne(c => c.Voucher)
                            .HasForeignKey(c => c.VoucherId)
                            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                           .HasOne(c => c.Buyer)
                           .WithMany(u => u.Carts)
                           .HasForeignKey(c => c.BuyerId)
                           .OnDelete(DeleteBehavior.Cascade); // Enable cascade delete

            modelBuilder.Entity<User>()
                            .HasMany(u => u.Carts)
                            .WithOne(c => c.Buyer)
                            .OnDelete(DeleteBehavior.Cascade); // Prevent cascade on the other side

            //modelBuilder.Entity<Report>()
            //                .HasMany(u => u.Medias)
            //                .WithOne(c => c.Report)
            //                .OnDelete(DeleteBehavior.Cascade); // Prevent cascade on the other side

            //modelBuilder.Entity<RefundRequest>()
            //                .HasMany(u => u.Medias)
            //                .WithOne(c => c.RefundRequest)
            //                .OnDelete(DeleteBehavior.Cascade); // Prevent cascade on the other side

            //modelBuilder.Entity<Rating>()
            //                .HasMany(u => u.Medias)
            //                .WithOne(c => c.Rating)
            //                .OnDelete(DeleteBehavior.Cascade); // Prevent cascade on the other side

            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }
    }
}
