using KTSite.Models;
using System;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IInventoryOrdersToAmazonRepository
    {
       public List<InventoryOrdersToAmazon> GetList();
       public InventoryOrdersToAmazon GetById(int id);
       public bool getInboundUpdated(string asin);
       public bool InsertInvOrder(string productAsin, string productSku, int quantity, DateTime dateOrdered, int lineNumber);
public int updateById(int Id, int quantity, DateTime dateReceived,bool inboundUpdated);
       
    }
}
