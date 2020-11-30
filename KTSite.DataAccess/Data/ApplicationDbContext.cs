using System;
using System.Collections.Generic;
using System.Text;
using KTSite.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KTSite.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public ApplicationDbContext()
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<UserStoreName> UserStoreNames { get; set; }
        public DbSet<SellersInventory> SellersInventories { get; set; }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<PaymentBalance> PaymentBalances { get; set; }
        public DbSet<PaymentHistory> PaymentHistories { get; set; }
        public DbSet<PaymentSentAddress> PaymentSentAddresses { get; set; }
        public DbSet<Complaints> Complaints { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<ChinaOrder> ChinaOrders { get; set; }
        public DbSet<ReturningItem> ReturningItems { get; set; }
        public DbSet<ReturnLabel> returnLabels { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserGuideline> UserGuidelines { get; set; }
        public DbSet<ArrivingFromChina> arrivingFromChinas { get; set; }
        public DbSet<AdminVATask> adminVaTasks { get; set; }

    }
}
