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
    public class AsinToSkuRepository :  IAsinToSkuRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public AsinToSkuRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<AsinToSku> GetList()
        {
         
            var sql =
                "SELECT a.Id, a.Asin, a.Sku, a.ChinaName ,a.RestockUS, a.RestockCA , a.ImageUrl," +
"                       a.RestockNOTDECIDED,a.IsCanadaAsin " + 
                " FROM AsinToSku a";

            List<AsinToSku> lst = _db.Query<AsinToSku>(sql).ToList();
            return lst;
        }
        public AsinToSku GetById(int id)
        {
         
            var sql =
                "SELECT a.Id, a.Asin, a.Sku, a.ChinaName ,a.RestockUS, a.RestockCA , a.ImageUrl," +
"                       a.RestockNOTDECIDED,a.IsCanadaAsin " + 
                " FROM AsinToSku a WHERE a.Id = " + id;

            AsinToSku lst = _db.Query<AsinToSku>(sql).Single();
            return lst;
        }
        public string GetSkuByAsin(string asin)
        {
         
            var sql =
                "SELECT a.Sku" + 
                " FROM AsinToSku a WHERE a.Asin = '" + asin + "'";

            string sku = _db.Query<string>(sql).Single();
            if (sku != null)
                 return sku;
            return null;
        }

        public int updateRestockStatus(string sql)
        {
            try
            {
                return _db.Execute(sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int updateById(int id , string asin, string sku, string chinaName, string imageUrl,
                              bool restockNOTDECIDED, bool IsCanadaAsin)
        {
            try
            {
                var sql =  " UPDATE AsinToSku " +
  "                               SET Asin = '"+asin+"' , Sku = '"+sku+"', " +
"                                     ChinaName = '"+chinaName+"', " +
"                                     ImageUrl = '"+imageUrl+"' ," +
"                                     RestockNOTDECIDED = "+ (restockNOTDECIDED ? 1 : 0)+", "+
"                                     IsCanadaAsin = "+ (IsCanadaAsin ? 1 : 0) + " "+  
  "                               WHERE Id = " + id; 
                return _db.Execute(sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
public bool InsertAsinToSku(string asin, string sku, string chinaName, string imageUrl)
        {
          try
            {
              var sql =  
                "INSERT INTO AsinToSku (Asin, Sku, ChinaName, RestockCA,RestockUS,ImageUrl)VALUES" +
              "('"+asin+"' , '"+sku+"', '"+chinaName+"',1,1, '"+imageUrl+"') ";
                 var id = _db.Query<int>(sql);
              return true ;
             }
            catch (Exception ex)
            {
                return false;
            }
        }
public bool DeleteById(int id)
        {
          try
            {
              var sql =  "DELETE From AsinToSku WHERE Id = @id";
              int effectedRows = _db.Execute(sql, new{id});
              return effectedRows >0 ;
             }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
