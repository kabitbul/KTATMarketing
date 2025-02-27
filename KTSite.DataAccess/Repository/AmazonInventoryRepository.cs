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
    public class AmazonInventoryRepository :  IAmazonInventoryRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public AmazonInventoryRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<AmazonInvStatistics> GetInventoryStat(string marketPlace)
        {
   var sql =
"SELECT sk.ImageUrl, sk.Sku, sk.Asin, "+
"            inv.AvailableQty AmzAvailQty, "+
"            (inv.InboundReceivingQty + inv.InboundShippedQty + inv.ReservedQty) AmzInboundQty, "+
"            sk.RestockUS restockUS, sk.RestockCA restockCA, "+
"			COALESCE((select sum(Quantity) from InventoryOrdersToAmazons ioa where ioa.ProductAsin = inv.Asin and ioa.InboundUpdated = 0),0) onTheWay "+
"     FROM AmazonInventories inv JOIN AsinToSku sk ON inv.Asin = sk.Asin "+
"     WHERE inv.MarketPlace = '"+marketPlace+"'";

            return _db.Query<AmazonInvStatistics>(sql).ToList();
        }
    }
}
