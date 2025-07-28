using Dapper;
using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Http.ModelBinding;

namespace KTSite.DataAccess.Repository
{
    public class AAmzStockPurchaseRepository :  IAAmzStockPurchaseRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public AAmzStockPurchaseRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<AAmzStockPurchase> GetList(int storeId,string marketplace)
        {

            var sql =
     "SELECT a.Id, a.ProductAsin, "+  
     "       ProductChinaName, "+
     "       a.Quantity ,a.DateOrdered, a.DateReceived , a.InboundUpdated,a.lineNumber "+
     "            FROM  AAmzStockPurchase a "+
     "				 where StoreId = "+storeId +" AND a.MarketPlace = '"+marketplace+"'";

            List<AAmzStockPurchase> lst = _db.Query<AAmzStockPurchase>(sql).ToList();
            return lst;
        }
public AAmzStockPurchase GetById(int id)
        {

            var sql =
     "SELECT a.Id, a.storeId, a.MarketPlace, a.ProductAsin, "+  
     "       ProductChinaName, "+
     "       a.Quantity ,a.DateOrdered, a.DateReceived , a.InboundUpdated,a.lineNumber "+
     "            FROM  AAmzStockPurchase a "+
     "				 where Id = "+id;
     AAmzStockPurchase lst = _db.Query<AAmzStockPurchase>(sql).Single();
            return lst;
        }
 public bool AddStock(AAmzStockPurchaseVM aAmzStockPurchaseVM)
    {
          try
            {
             int storeId = aAmzStockPurchaseVM.aAmzStockPurchase.StoreId;
             string marketplace =aAmzStockPurchaseVM.aAmzStockPurchase.MarketPlace;
             string productAsin = aAmzStockPurchaseVM.aAmzStockPurchase.ProductAsin;
             string chinaName = aAmzStockPurchaseVM.aAmzStockPurchase.ProductChinaName;
             int qty = aAmzStockPurchaseVM.aAmzStockPurchase.Quantity;
              var dateOrd = aAmzStockPurchaseVM.aAmzStockPurchase.DateOrdered.ToString("yyyy-MM-dd HH:mm:ss");
             var dateRecieve = aAmzStockPurchaseVM.aAmzStockPurchase.DateReceived.ToString("yyyy-MM-dd HH:mm:ss");
             bool inboundUpdate = aAmzStockPurchaseVM.aAmzStockPurchase.InboundUpdated;
             int lineNumber = aAmzStockPurchaseVM.aAmzStockPurchase.lineNumber;
              var sql =  
   "INSERT INTO AAmzStockPurchase (StoreId, MarketPlace, ProductAsin, ProductChinaName,Quantity,DateOrdered,DateReceived,InboundUpdated,lineNumber)VALUES" +
   "("+storeId+" , '"+marketplace+"'," +" '"+productAsin+"', '"+chinaName+"'," +""+qty+",'"+dateOrd+"','"+dateRecieve+"'," +
     (inboundUpdate ? "1" : "0") +","+lineNumber+") ";
                 var id = _db.Query<int>(sql);
              return true ;
             }
            catch (Exception ex)
            {
                return false;
            }
        }    
public int updateById(AAmzStockPurchaseVM aAmzStockPurchaseVM)
        {
            try
            {
               int qty = aAmzStockPurchaseVM.aAmzStockPurchase.Quantity;
               bool inboundUpdate = aAmzStockPurchaseVM.aAmzStockPurchase.InboundUpdated;
               var dateRecieve = aAmzStockPurchaseVM.aAmzStockPurchase.DateReceived.ToString("yyyy-MM-dd HH:mm:ss");
                int inbUpd = 0;
               if (inboundUpdate)
                   inbUpd = 1;
               else
                  inbUpd = 0;
                var sql =  " UPDATE AAmzStockPurchase " +
  "                               SET Quantity = "+qty+" , InboundUpdated = "+inbUpd+", DateReceived = '" +dateRecieve+"' "+ 
  "                               WHERE Id = " + aAmzStockPurchaseVM.aAmzStockPurchase.Id; 
                return _db.Execute(sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
