using KTSite.Models;
using System;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IInventoryOrdersToAmzCARepository
    {
       public List<InventoryOrdersToAmzCA> GetList();
       public InventoryOrdersToAmzCA GetById(int id);
       public bool getInboundUpdated(string asin);
       public bool InsertInvOrder(string productAsin, string productSku, int quantity, DateTime dateOrdered);
public int updateById(int Id, int quantity, DateTime dateReceived,bool inboundUpdated);

       
    }
}
