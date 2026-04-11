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
    public class AAmzInventoryCostRepository :  IAAmzInventoryCostRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public AAmzInventoryCostRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<AAmzInventoryCost> GetList(int storeId, string marketplace, int days)
        {
         
            var sql = "SELECT Id, StoreId, MarketPlace," +
"                             a.DateCreated,FBACost,AWDCost,OnTheWayCost " +
"                      FROM aamzInventoryCost a " +
"                      WHERE a.storeId = "+storeId + " AND a.marketplace = '"+marketplace+"'" +
"                         AND a.DateCreated >= dateadd(day, -"+days+" , getdate())";

            List<AAmzInventoryCost> lst = _db.Query<AAmzInventoryCost>(sql).ToList();
            return lst;
        }
        public  List<AAmzInventoryCost> GetByLastDate(int storeId)
        {
            var sql = "WITH usRec as (SELECT TOP 1 Id, StoreId, MarketPlace," +
"                                            a.DateCreated,FBACost,AWDCost,OnTheWayCost" +
"                                    FROM aamzInventoryCost a " +
"                                    WHERE a.storeId = "+storeId +"  and a.marketPlace = 'US'" +
"                                    ORDER BY DateCreated desc)," +
"                         caRec as(SELECT TOP 1 Id, StoreId, MarketPlace," +
"                                         a.DateCreated,FBACost,AWDCost,OnTheWayCost" +
"                                  FROM aamzInventoryCost a " +
"                                  WHERE a.storeId = "+storeId + " and a.marketPlace = 'CA'" +
"                                  ORDER BY DateCreated desc)" +
"                 SELECT * FROM usRec" +
"                 UNION" +
"                 SELECT * FROM caRec";         

            
            List<AAmzInventoryCost> lst = _db.Query<AAmzInventoryCost>(sql).ToList();
            return lst;
        }
    }
}
