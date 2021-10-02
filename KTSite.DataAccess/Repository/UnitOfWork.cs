using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;

namespace KTSite.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            UserStoreName = new UserStoreNameRepository(_db);
            SellersInventory = new SellersInventoryRepository(_db);
            Order = new OrderRepository(_db);
            OrderArchive = new OrderArchiveRepository(_db);
            PaymentHistory = new PaymentHistoryRepository(_db);
            PaymentHistoryArchive = new PaymentHistoryArchiveRepository(_db);
            PaymentBalance = new PaymentBalanceRepository(_db);
            PaymentHistoryMerch = new PaymentHistoryMerchRepository(_db);
            PaymentBalanceMerch = new PaymentBalanceMerchRepository(_db);
            PaymentSentAddress = new PaymentSentAddressRepository(_db);
            Complaints = new ComplaintsRepository(_db);
            ComplaintsArchive = new ComplaintsArchiveRepository(_db);
            Refund = new RefundRepository(_db);
            RefundArchive = new RefundArchiveRepository(_db);
            ChinaOrder = new ChinaOrderRepository(_db);
            ReturningItem = new ReturningItemRepository(_db);
            ReturningItemArchive = new ReturningItemArchiveRepository(_db);
            ReturnLabel = new ReturnLabelRepository(_db);
            ReturnLabelArchive = new ReturnLabelArchiveRepository(_db);
            Notification = new NotificationRepository(_db);
            UserGuideline = new UserGuidelineRepository(_db);
            ArrivingFromChina = new ArrivingFromChinaRepository(_db);
            adminVATask = new AdminVATaskRepository(_db);
            paymentBalanceBackup = new PaymentBalanceBackupRepository(_db);
            paymentMethodMerch = new PaymentMethodMerchRepository(_db);
            SP_Call = new SP_Call(_db);
            logsData = new LogsDataRepository(_db);
        }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IUserStoreNameRepository UserStoreName { get; private set; }
        public ISellersInventoryRepository SellersInventory { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IOrderArchiveRepository OrderArchive { get; private set; }
        public IPaymentHistoryRepository PaymentHistory { get; private set; }
        public IPaymentHistoryArchiveRepository PaymentHistoryArchive { get; private set; }
        public IPaymentBalanceRepository PaymentBalance { get; private set; }
        public IPaymentHistoryMerchRepository PaymentHistoryMerch { get; private set; }
        public IPaymentBalanceMerchRepository PaymentBalanceMerch { get; private set; }
        public IPaymentSentAddressRepository PaymentSentAddress { get; private set; }
        public IComplaintsRepository Complaints { get; private set; }
        public IComplaintsArchiveRepository ComplaintsArchive { get; private set; }
        public IRefundRepository Refund { get; private set; }
        public IRefundArchiveRepository RefundArchive { get; private set; }
        public IChinaOrderRepository ChinaOrder { get; private set; }
        public IReturningItemRepository ReturningItem { get; private set; }
        public IReturningItemArchiveRepository ReturningItemArchive { get; private set; }
        public IReturnLabelRepository ReturnLabel { get; private set; }
        public IReturnLabelArchiveRepository ReturnLabelArchive { get; private set; }
        public INotificationRepository Notification { get; private set; }
        public IUserGuidelineRepository UserGuideline { get; private set; }
        public IArrivingFromChinaRepository ArrivingFromChina { get; private set; }
        public IAdminVATaskRepository adminVATask { get; private set; }
        public IPaymentBalanceBackupRepository paymentBalanceBackup { get; private set; }
        public IPaymentMethodMerchRepository paymentMethodMerch { get; private set; }
        public ISP_Call SP_Call { get; private set; }
        public ILogsDataRepository logsData { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
