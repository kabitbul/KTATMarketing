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
    public class LitalInventoryOrdersToAmazonRepository :  ILitalInventoryOrdersToAmazonRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public LitalInventoryOrdersToAmazonRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<LitalInventoryOrdersToAmazon> GetList()
        {
         
            var sql =
                "SELECT a.Id, a.ProductAsin,  " +
"                   (SELECT ats.ChinaName FROM LitalAsinToSku ats WHERE ats.Asin = a.ProductAsin) ProductChina," +
"                 a.Quantity ,a.DateOrdered, a.DateReceived , a.InboundUpdated" + 
                " FROM  LitalInventoryOrdersToAmazon a";

            List<LitalInventoryOrdersToAmazon> lst = _db.Query<LitalInventoryOrdersToAmazon>(sql).ToList();
            return lst;
        }
        public LitalInventoryOrdersToAmazon GetById(int id)
        {
         
            var sql =
                "SELECT a.Id, a.ProductAsin, a.Quantity ,a.DateOrdered, a.DateReceived , a.InboundUpdated" + 
                " FROM LitalInventoryOrdersToAmazon a WHERE a.Id = " + id;

            LitalInventoryOrdersToAmazon lst = _db.Query<LitalInventoryOrdersToAmazon>(sql).Single();
            return lst;
        }
        public bool getInboundUpdated(string asin)
        {
          var sql = @"
            SELECT COUNT(*) 
            FROM LitalInventoryOrdersToAmazon
            WHERE ProductAsin = @asin AND InboundUpdated = 0";
        
    int count = _db.Query<int>(sql, new { asin }).Single();
    return count == 0;
        }

public bool InsertInvOrder(string productAsin, int quantity, DateTime dateOrdered)
        {
          try
            {
              var formattedDate = dateOrdered.ToString("yyyy-MM-dd HH:mm:ss");
             var maxDate = DateTime.MaxValue.ToString("yyyy-MM-dd HH:mm:ss");
              var sql =  
                "INSERT INTO LitalInventoryOrdersToAmazon (ProductAsin, Quantity, DateOrdered,DateReceived,InboundUpdated)VALUES" +
              "('"+productAsin+"' , '"+quantity+"', '"+formattedDate+"','"+maxDate+"',0) ";
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
                var sql =  " UPDATE LitalInventoryOrdersToAmazon " +
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
