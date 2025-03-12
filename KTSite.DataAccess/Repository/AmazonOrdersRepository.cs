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
    public class AmazonOrdersRepository :  IAmazonOrdersRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public AmazonOrdersRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<SkuQtyForAverage> GetAllOrdersForAvg(string marketPlace)
        {
         string startDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow.AddDays(-32),
                              TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
                          .Date.ToString("yyyy-MM-dd HH:mm:ss");
       string endDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow,
                              TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
                          .Date.ToString("yyyy-MM-dd HH:mm:ss");
            var sql =
                "SELECT sk.Asin, o.Qty,o.PurchaseDate "+
"                FROM AmazonOrders o , AsinToSku sk " +
"                WHERE MarketPlace = '"+marketPlace+"' AND " +
"                      o.Asin = sk.Asin AND "+
"                      o.PurchaseDate >=  '"+startDate+"' AND" +
"                      o.PurchaseDate <= '"+endDate+"'";

            List<SkuQtyForAverage> ordList = _db.Query<SkuQtyForAverage>(sql).ToList();
            return ordList;
        } 
       public List<GraphData> GetGraphData(string marketPlace, string asin)
        {

            var sql =
"                WITH Months AS (SELECT "+ 
"                YEAR(DATEADD(MONTH, -n, DATEFROMPARTS(YEAR(GETDATE()),  " +
"                                        MONTH(GETDATE()) - 1, 1))) AS pyear, "+
"                 MONTH(DATEADD(MONTH, -n, " +
"                       DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()) - 1, 1))) AS pmonth "+
"    FROM ( "+
"        SELECT TOP 12 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n  "+
"        FROM sys.all_objects "+
"    ) t "+
") "+
"   SELECT m.pyear,m.pmonth, COALESCE(SUM(t10.qty), 0) AS totalQty "+
"   FROM Months m "+
"   LEFT JOIN ( SELECT MONTH(PurchaseDate) AS pmonth,  "+
"                      YEAR(PurchaseDate) AS pyear, "+ 
"                a.qty "+
"    FROM AmazonOrders a "+
"    WHERE purchaseDate >= DATEADD(YEAR, -1, DATEFROMPARTS(YEAR(GETDATE()), " +
"                                   MONTH(GETDATE()) - 1, 1)) "+ 
"          AND purchaseDate < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) "+ 
"          AND a.Asin = '"+asin+"' "+ 
"          AND a.MarketPlace = '"+marketPlace+"' "+
" ) t10 "+
"  ON m.pyear = t10.pyear AND m.pmonth = t10.pmonth "+
"  GROUP BY m.pyear, m.pmonth "+
"  ORDER BY m.pyear, m.pmonth ";


            List<GraphData> ordList = _db.Query<GraphData>(sql).ToList();
            return ordList;
        }
public List<GraphData> GetTotalOrdGraphData(string marketPlace)
        {

            var sql =
"                WITH Months AS (SELECT "+ 
"                YEAR(DATEADD(MONTH, -n, DATEFROMPARTS(YEAR(GETDATE()),  " +
"                                        MONTH(GETDATE()) - 1, 1))) AS pyear, "+
"                 MONTH(DATEADD(MONTH, -n, " +
"                       DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()) - 1, 1))) AS pmonth "+
"    FROM ( "+
"        SELECT TOP 12 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n  "+
"        FROM sys.all_objects "+
"    ) t "+
") "+
"   SELECT m.pyear,m.pmonth, COALESCE(SUM(t10.qty), 0) AS totalQty "+
"   FROM Months m "+
"   LEFT JOIN ( SELECT MONTH(PurchaseDate) AS pmonth,  "+
"                      YEAR(PurchaseDate) AS pyear, "+ 
"                a.qty "+
"    FROM AmazonOrders a "+
"    WHERE purchaseDate >= DATEADD(YEAR, -1, DATEFROMPARTS(YEAR(GETDATE()), " +
"                                   MONTH(GETDATE()) - 1, 1)) "+ 
"          AND purchaseDate < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) "+  
"          AND a.MarketPlace = '"+marketPlace+"' "+
" ) t10 "+
"  ON m.pyear = t10.pyear AND m.pmonth = t10.pmonth "+
"  GROUP BY m.pyear, m.pmonth "+
"  ORDER BY m.pyear, m.pmonth ";


            List<GraphData> ordList = _db.Query<GraphData>(sql).ToList();
            return ordList;
        }
    }
}
