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
    public class AAmzAWDInventoryRepository :  IAAmzAWDInentoryRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public AAmzAWDInventoryRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<AmazonInvStatistics> GetInventoryStat(string marketPlace,bool? showRestock)
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
"SELECT sk.Id,sk.ImageUrl, sk.Sku, sk.Asin, sk.ChinaName, "+
"            inv.AvailableQty AmzAvailQty, "+
"            (inv.InboundReceivingQty + inv.InboundShippedQty + inv.ReservedQty) AmzInboundQty, "+
"            aw.totalInboundQuantity AmzAWDInboundQty, aw.totalOnhandQuantity AmzAWDAvailQty, "+
"            sk.RestockUS restockUS, sk.RestockCA restockCA,sk.RestockNOTDECIDED, "+
"			COALESCE((select sum(Quantity) from InventoryOrdersToAmazons ioa "+
"			                        where ioa.ProductAsin = inv.Asin and ioa.InboundUpdated = 0),0) onTheWay "+
"     FROM AmazonInventories inv JOIN AsinToSku sk ON inv.Asin = sk.Asin left join AmazonAWDInventories aw on aw.Asin = sk.Asin"+
"     WHERE inv.MarketPlace = '"+marketPlace+"'";
      if(shr)
       { 
     sql = sql + " AND RestockUS = 1 ";
        }    
}
else{//CA
sql =
"SELECT sk.Id,sk.ImageUrl, sk.Sku, sk.Asin, sk.ChinaName,"+
"            inv.AvailableQty AmzAvailQty, "+
"            (inv.InboundReceivingQty + inv.InboundShippedQty + inv.ReservedQty) AmzInboundQty, "+
"            sk.RestockUS restockUS, sk.RestockCA restockCA, sk.RestockNOTDECIDEDCA, "+
"			COALESCE((select sum(Quantity) from InventoryOrdersToAmzCA ioa where ioa.ProductAsin = inv.Asin and ioa.InboundUpdated = 0),0) onTheWay "+
"     FROM AmazonInventories inv JOIN AsinToSku sk ON inv.Asin = sk.Asin "+
"     WHERE inv.MarketPlace = '"+marketPlace+"' and sk.IsCanadaAsin = 1";
if(shr)
       { 
     sql = sql + " AND RestockCA = 1 ";
        }
            }
     
            return _db.Query<AmazonInvStatistics>(sql).ToList();
        }
    }
}
