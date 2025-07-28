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
    public class AAmazonStoresRepository :  IAAmazonStoresRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public AAmazonStoresRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<AAmazonStores> GetList()
        {
         
            var sql =
                "SELECT a.Id,a.StoreName FROM AAmazonStores a";

            List<AAmazonStores> lst = _db.Query<AAmazonStores>(sql).ToList();
            return lst;
        }
        public AAmazonStores GetById(int id)
        {
         
            var sql =
                "SELECT a.Id,a.StoreName FROM AAmazonStores a WHERE a.Id = " + id;

            AAmazonStores lst = _db.Query<AAmazonStores>(sql).Single();
            return lst;
        }

        public int updateById(int id , string storeName)
        {
            try
            {
                var sql =  " UPDATE AAmazonStores " +
  "                               SET StoreName = '"+storeName+"' WHERE Id = " + id; 
                return _db.Execute(sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
public bool InsertStore(string storeName)
        {
          try
            {
              var sql =  
                "INSERT INTO AAmazonStores (StoreName) VALUES ('"+storeName+"') ";
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
              var sql =  "DELETE From AAmazonStores WHERE Id = @id";
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
