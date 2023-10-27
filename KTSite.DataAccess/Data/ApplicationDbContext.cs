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
        public DbSet<OrderArchive> OrdersArchive { get; set; }
        public DbSet<PaymentBalance> PaymentBalances { get; set; }
        public DbSet<PaymentHistory> PaymentHistories { get; set; }
        public DbSet<PaymentHistoryArchive> PaymentHistoriesArchive { get; set; }
        public DbSet<PaymentBalanceMerch> PaymentBalancesMerch { get; set; }
        public DbSet<PaymentHistoryMerch> PaymentHistoriesMerch { get; set; }
        public DbSet<PaymentSentAddress> PaymentSentAddresses { get; set; }
        public DbSet<Complaints> Complaints { get; set; }
        public DbSet<ComplaintsArchive> ComplaintsArchive { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<RefundArchive> RefundsArchive { get; set; }
        public DbSet<ChinaOrder> ChinaOrders { get; set; }
        public DbSet<ReturningItem> ReturningItems { get; set; }
        public DbSet<ReturningItemArchive> ReturningItemsArchive { get; set; }
        public DbSet<ReturnLabel> returnLabels { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ReturnLabelArchive> returnLabelsArchive { get; set; }
        public DbSet<UserGuideline> UserGuidelines { get; set; }
        public DbSet<ArrivingFromChina> arrivingFromChinas { get; set; }
        public DbSet<AdminVATask> adminVaTasks { get; set; }
        public DbSet<PaymentBalanceBackup> PaymentBalanceBackups { get; set; }
        public DbSet<PaymentMethodMerch> PaymentMethodMerchs { get; set; }
        public DbSet<LogsData> LogsDatas { get; set; }
        public DbSet<ExcelUploadsForShops> ExcelUploadsForShopss { get; set; }
        public DbSet<UsersForAPI> UsersForAPIs { get; set; }
        public DbSet<BalanceUpdate> BalanceUpdates { get; set; }
        public DbSet<InventoryOnTexas> InventoriesOnTexas { get; set; }
        public DbSet<AsinToSku> AsinToSku { get; set; }
        public DbSet<AmazonOrders> AmazonOrders { get; set; }
        public DbSet<AmazonInventory> AmazonInventories { get; set; }
    }
}
