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
using System.Text.RegularExpressions;

namespace KTSite.DataAccess.Repository
{
    public class LitalAmazonInventoryRepository :  ILitalAmazonInventoryRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public LitalAmazonInventoryRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<LitalAmazonInvStatistics> GetInventoryStat(string marketPlace,bool? showRestock)
        {
            string sql;
            bool shr;
            if (showRestock == null || showRestock == true)
                 shr = true;
            else
             shr = false;
           if (marketPlace == "US")
            { 
    sql =
"SELECT sk.Id,sk.ImageUrl, sk.Asin, sk.ChinaName,"+
"            inv.AvailableQty AmzAvailQty, "+
"            (inv.InboundReceivingQty + inv.InboundShippedQty + inv.ReservedQty) AmzInboundQty, "+
"            aw.totalInboundQuantity AmzAWDInboundQty, aw.totalOnhandQuantity AmzAWDAvailQty, "+
"            sk.Restock restock, sk.RestockNOTDECIDED, "+
"			COALESCE((select sum(Quantity) from LitalInventoryOrdersToAmazon ioa where ioa.ProductAsin = inv.Asin and ioa.InboundUpdated = 0),0) onTheWay "+
"     FROM LitalAmazonInventories inv JOIN LitalAsinToSku sk ON inv.Asin = sk.Asin left join LitalAmazonAWDInventories aw on aw.Asin = sk.Asin"+
"     WHERE inv.MarketPlace = '"+marketPlace+"'";
      if(shr)
       { 
     sql = sql + " AND Restock = 1 ";
        }    
       return _db.Query<LitalAmazonInvStatistics>(sql).ToList();
     }
     return null;
            
        }
    }
}
