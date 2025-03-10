﻿using System;
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
        IOrderArchiveRepository OrderArchive { get; }
        IPaymentBalanceRepository PaymentBalance { get; }
        IPaymentHistoryRepository PaymentHistory { get; }
        IPaymentHistoryArchiveRepository PaymentHistoryArchive { get; }
        IPaymentSentAddressRepository PaymentSentAddress { get; }
        IPaymentBalanceMerchRepository PaymentBalanceMerch { get; }
        IPaymentHistoryMerchRepository PaymentHistoryMerch { get; }
        IComplaintsRepository Complaints { get; }
        IComplaintsArchiveRepository ComplaintsArchive { get; }
        IRefundRepository Refund { get; }
        IRefundArchiveRepository RefundArchive { get; }
        IChinaOrderRepository ChinaOrder { get; }
        IReturningItemRepository ReturningItem { get; }
        IReturningItemArchiveRepository ReturningItemArchive { get; }
        IReturnLabelRepository ReturnLabel { get; }
        IReturnLabelArchiveRepository ReturnLabelArchive { get; }
        INotificationRepository Notification { get; }
        IUserGuidelineRepository UserGuideline { get; }
        IArrivingFromChinaRepository ArrivingFromChina { get; }
        IAdminVATaskRepository adminVATask { get; }
        IPaymentBalanceBackupRepository paymentBalanceBackup { get; }
        IPaymentMethodMerchRepository paymentMethodMerch { get; }
        IBalanceUpdateRepository balanceUpdate { get; }
        ISP_Call SP_Call { get; }
        ILogsDataRepository logsData  { get; }
        IExcelUploadsForShopsRepository excelUploadsForShops { get; }
        IUsersForAPIRepository UsersForAPI { get; }
        IInventoryOnTexasRepository inventoryOnTexas { get; }
        IAsinToSkuRepository asinToSku { get; }
        IAmazonOrdersRepository amazonOrders { get; }
        IAmazonInventoryRepository amazonInventories { get; }
        IInventoryOrdersToAmazonRepository inventoryOrdersToAmazon { get; }
        IInventoryOrdersToAmzCARepository inventoryOrdersToAmzCA { get; }
        ILitalAmazonOrdersRepository litalAmazonOrders { get; }
        ILitalAsinToSkuRepository litalAsinToSku { get; }
        ILitalInventoryOrdersToAmazonRepository litalInventoryOrdersToAmazon { get; }
        ILitalAmazonInventoryRepository litalAmazonInventories { get; }
        void Save();
    }
}
