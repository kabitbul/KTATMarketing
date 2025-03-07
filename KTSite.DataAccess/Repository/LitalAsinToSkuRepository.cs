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
    public class LitalAsinToSkuRepository :  ILitalAsinToSkuRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public LitalAsinToSkuRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<LitalAsinToSku> GetList()
        {
         
            var sql =
                "SELECT a.Id, a.Asin, a.ChinaName ,a.Restock , a.ImageUrl," +
"                       a.RestockNOTDECIDED " + 
                " FROM LitalAsinToSku a";

            List<LitalAsinToSku> lst = _db.Query<LitalAsinToSku>(sql).ToList();
            return lst;
        }
        public LitalAsinToSku GetById(int id)
        {
         
            var sql =
                "SELECT a.Id, a.Asin,  a.ChinaName ,a.Restock , a.ImageUrl," +
"                       a.RestockNOTDECIDED " + 
                " FROM LitalAsinToSku a WHERE a.Id = " + id;

            LitalAsinToSku lst = _db.Query<LitalAsinToSku>(sql).Single();
            return lst;
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
        public int updateById(int id , string asin, string chinaName, string imageUrl,
                              bool restockNOTDECIDED)
        {
            try
            {
                var sql =  " UPDATE LitalAsinToSku " +
  "                               SET Asin = '"+asin+"' , " +
"                                     ChinaName = '"+chinaName+"', " +
"                                     ImageUrl = '"+imageUrl+"' ," +
"                                     RestockNOTDECIDED = "+ (restockNOTDECIDED ? 1 : 0)+" "+
  "                               WHERE Id = " + id; 
                return _db.Execute(sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
public bool InsertAsinToSku(string asin, string chinaName, string imageUrl)
        {
          try
            {
              var sql =  
                "INSERT INTO LitalAsinToSku (Asin, ChinaName, Restock,ImageUrl)VALUES" +
              "('"+asin+"' ,  '"+chinaName+"',1, '"+imageUrl+"') ";
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
              var sql =  "DELETE From LitalAsinToSku WHERE Id = @id";
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
