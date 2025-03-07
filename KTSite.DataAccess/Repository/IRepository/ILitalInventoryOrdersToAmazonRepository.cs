using KTSite.Models;
using System;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface ILitalInventoryOrdersToAmazonRepository
    {
       public List<LitalInventoryOrdersToAmazon> GetList();
       public LitalInventoryOrdersToAmazon GetById(int id);
       public bool getInboundUpdated(string asin);
       public bool InsertInvOrder(string productAsin, int quantity, DateTime dateOrdered);
public int updateById(int Id, int quantity, DateTime dateReceived,bool inboundUpdated);
       
    }
}
