using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        IApplicationUserRepository ApplicationUser { get; }

        IUserStoreNameRepository UserStoreName { get; }
        ISellersInventoryRepository SellersInventory { get; }
        IOrderRepository Order { get; }
        IPaymentBalanceRepository PaymentBalance { get; }
        IPaymentHistoryRepository PaymentHistory { get; }
        IPaymentSentAddressRepository PaymentSentAddress { get; }
        IComplaintsRepository Complaints { get; }
        IRefundRepository Refund { get; }
        IChinaOrderRepository ChinaOrder { get; }
        IReturningItemRepository ReturningItem { get; }
        IReturnLabelRepository ReturnLabel { get; }
        INotificationRepository Notification { get; }
        IUserGuidelineRepository UserGuideline { get; }
        IArrivingFromChinaRepository ArrivingFromChina { get; }
        ISP_Call SP_Call { get; }

        void Save();
    }
}
