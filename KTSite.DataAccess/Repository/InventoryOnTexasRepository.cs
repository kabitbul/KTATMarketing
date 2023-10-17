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
    public class InventoryOnTexasRepository :  IInventoryOnTexasRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public InventoryOnTexasRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<InventoryOnTexasSumList> GetInventoryOnTexasSum()
        {
        var sql =
               //                "SELECT SKU, SUM(AvailableQty) totalInventory " +
               //"                FROM InventoriesOnTexas t ," +
               //"                     (SELECT day(DateCreated) dayC, " +
               //"                             month(DateCreated) monthC," +
               //"                             YEAR(DateCreated) yearC," +
               //"                             DATEPART(hour, DateCreated) hourC" +
               //"                         FROM InventoriesOnTexas " +
               //"                         WHERE Id = (SELECT MAX(Id) " +
               //"                                     FROM InventoriesOnTexas)) t1" +
               //"                 WHERE PalletId  > 0 AND" +
               //"                       day(t.DateCreated) = t1.dayC AND " +
               //"                       month(t.DateCreated) = t1.monthC AND YEAR(t.DateCreated) = t1.yearC AND" +
               //"                       DATEPART(hour, t.DateCreated) = t1.hourC" +
               //"                 GROUP BY SKU";
               "SELECT SKU, SUM(AvailableQty) totalInventory "+
               "FROM InventoriesOnTexas WHERE PalletId  > 0 "+
               "GROUP BY SKU";
            return _db.Query<InventoryOnTexasSumList>(sql).ToList();
        }
    }
}
