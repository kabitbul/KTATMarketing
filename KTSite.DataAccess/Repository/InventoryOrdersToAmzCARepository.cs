using Dapper;
using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class InventoryOrdersToAmzCARepository :  IInventoryOrdersToAmzCARepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public InventoryOrdersToAmzCARepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<InventoryOrdersToAmzCA> GetList()
        {
         
            var sql =
                "SELECT a.Id, a.ProductAsin, a.ProductSku, " +
  "                (SELECT ats.ChinaName FROM AsinToSku ats WHERE ats.Asin = a.ProductAsin) ProductChina," +
  "                a.Quantity ,a.DateOrdered, a.DateReceived , a.InboundUpdated,a.lineNumber" + 
                " FROM  InventoryOrdersToAmzCA a";

            List<InventoryOrdersToAmzCA> lst = _db.Query<InventoryOrdersToAmzCA>(sql).ToList();
            return lst;
        }
        public InventoryOrdersToAmzCA GetById(int id)
        {
         
            var sql =
                "SELECT a.Id, a.ProductAsin, a.ProductSku, a.Quantity ,a.DateOrdered, a.DateReceived , a.InboundUpdated,a.lineNumber" + 
                " FROM InventoryOrdersToAmzCA a WHERE a.Id = " + id;

            InventoryOrdersToAmzCA lst = _db.Query<InventoryOrdersToAmzCA>(sql).Single();
            return lst;
        }
        public bool getInboundUpdated(string asin)
        {
          var sql = @"
            SELECT COUNT(*) 
            FROM InventoryOrdersToAmzCA 
            WHERE ProductAsin = @asin AND InboundUpdated = 0";
        
    int count = _db.Query<int>(sql, new { asin }).Single();
    return count == 0;
        }

public bool InsertInvOrder(string productAsin, string productSku, int quantity, DateTime dateOrdered, int lineNumber)
        {
          try
            {
              var formattedDate = dateOrdered.ToString("yyyy-MM-dd HH:mm:ss");
             var maxDate = DateTime.MaxValue.ToString("yyyy-MM-dd HH:mm:ss");
              var sql =  
                "INSERT INTO InventoryOrdersToAmzCA (ProductAsin, ProductSku, Quantity, lineNumber,DateOrdered,DateReceived,InboundUpdated)VALUES" +
              "('"+productAsin+"' , '"+productSku+"', '"+quantity+"', '"+lineNumber+"','"+formattedDate+"','"+maxDate+"',0) ";
                 var id = _db.Query<int>(sql);
              return true ;
             }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int updateById(int Id, int quantity, DateTime dateReceived,bool inboundUpdated)
        {
            try
            {
               var formattedDate = dateReceived.ToString("yyyy-MM-dd HH:mm:ss");
                int inbUpd = 0;
               if (inboundUpdated)
                   inbUpd = 1;
               else
                  inbUpd = 0;
                var sql =  " UPDATE InventoryOrdersToAmzCA " +
  "                               SET Quantity = "+quantity+" , InboundUpdated = "+inbUpd+", DateReceived = '" +formattedDate+"' "+ 
  "                               WHERE Id = " + Id; 
                return _db.Execute(sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

    }
}
