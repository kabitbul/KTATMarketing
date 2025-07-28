using Dapper;
using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class AAmzAsinToSkuRepository :  IAAmzAsinToSkuRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        private IAAmzOrdersRepository _amazonOrdersRepository;
        public AAmzAsinToSkuRepository(IConfiguration configuration, IAAmzOrdersRepository amzord)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
         _amazonOrdersRepository = amzord;   
        }
        public List<AAmzAsinToSku> GetList(int store)
        {
            var sql =
                "SELECT a.Id,a.StoreId, a.Asin, a.Sku, " +
"                       a.ChinaName ,a.RestockUS, a.RestockCA , a.ImageUrl," +
"                       a.RestockNOTDECIDED,a.IsCanadaAsin " + 
                " FROM AAmzAsinToSku a WHERE a.StoreId = " + store;

            List<AAmzAsinToSku> lst = _db.Query<AAmzAsinToSku>(sql).ToList();
            return lst;
        }
public List<AAmzAsinToSku> GetListByMarketplace(int store,string marketplace)
        {
            var sql =
                "SELECT a.Id,a.StoreId, a.Asin, a.Sku, " +
"                       a.ChinaName ,a.RestockUS, a.RestockCA , a.ImageUrl," +
"                       a.RestockNOTDECIDED,a.IsCanadaAsin " + 
                " FROM AAmzAsinToSku a WHERE a.StoreId = " + store ;
           if (marketplace == SD.marketPlaceCA)
            {
              sql = sql + " AND IsCanadaAsin = 1";
            }
            List<AAmzAsinToSku> lst = _db.Query<AAmzAsinToSku>(sql).ToList();
            return lst;
        }

       public AAmzAsinToSku getForUpsert(int id,int storeId)
      {
      AAmzAsinToSku aAmzAsinToSku = new AAmzAsinToSku();
            if(id == 0)//create
            {
                return aAmzAsinToSku;
            }
            aAmzAsinToSku = GetById(id,storeId);
            if (aAmzAsinToSku == null)
            {
                return null;
            }
            return aAmzAsinToSku;
        }
      public bool Upsert(AAmzAsinToSku aAmzAsinToSku,int storeId)
      {
        if(aAmzAsinToSku.Id == 0)
                {
                    return InsertAsinToSku(storeId,aAmzAsinToSku.Asin,aAmzAsinToSku.Sku,
                               aAmzAsinToSku.ChinaName,aAmzAsinToSku.ImageUrl,aAmzAsinToSku.IsCanadaAsin);
                   
                }
                else
                {
                   int res = updateById(storeId,aAmzAsinToSku.Id,aAmzAsinToSku.Asin,aAmzAsinToSku.Sku,
                          aAmzAsinToSku.ChinaName,aAmzAsinToSku.ImageUrl,aAmzAsinToSku.RestockNOTDECIDED,
                          aAmzAsinToSku.IsCanadaAsin);
                  if (res != 1) 
                     return false;
                  return true;
                }
        }

        public AAmzAsinToSku GetById(int id, int storeId)
        {
         
            var sql =
                "SELECT a.Id, a.Asin, a.Sku, a.ChinaName ,a.RestockUS, a.RestockCA , a.ImageUrl," +
"                       a.RestockNOTDECIDED,a.IsCanadaAsin " + 
                " FROM AAmzAsinToSku a WHERE a.Id = " + id + " AND a.StoreId = " + storeId;

            AAmzAsinToSku lst = _db.Query<AAmzAsinToSku>(sql).Single();
            return lst;
        }
        public string GetSkuByAsin(string asin)
        {
         
            var sql =
                "SELECT a.ChinaName" + 
                " FROM AAmzAsinToSku a WHERE a.Asin = '" + asin + "'";

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
        public int updateById(int storeId,int id , string asin, string sku, string chinaName, string imageUrl,
                              bool restockNOTDECIDED, bool IsCanadaAsin)
        {
            try
            {
                var sql =  " UPDATE AAmzAsinToSku " +
  "                               SET Asin = '"+asin+"' , Sku = '"+sku+"', " +
"                                     ChinaName = '"+chinaName+"', " +
"                                     ImageUrl = '"+imageUrl+"' ," +
"                                     RestockNOTDECIDED = "+ (restockNOTDECIDED ? 1 : 0)+", "+
"                                     IsCanadaAsin = "+ (IsCanadaAsin ? 1 : 0) + " "+  
  "                               WHERE Id = " + id +" AND StoreId = " + storeId; 
                return _db.Execute(sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
public bool InsertAsinToSku(int storeId,string asin, string sku, string chinaName, string imageUrl,bool IsCanadaAsin)
        {
          try
            {
              var sql =  
                "INSERT INTO AAmzAsinToSku (StoreId,Asin, Sku, ChinaName, RestockCA,RestockUS,ImageUrl,IsCanadaAsin,RestockNOTDECIDED,RestockNOTDECIDEDCA)VALUES" +
              "("+storeId+" , '"+asin+"' , '"+sku+"', '"+chinaName+"',1,1, '"+imageUrl+"',"+(IsCanadaAsin ? 1 : 0)+",0,0) ";
                 var id = _db.Query<int>(sql);
              return true ;
             }
            catch (Exception ex)
            {
                return false;
            }
        }
public bool DeleteById(int id, int storeId)
        {
          try
            {
              var sql =  "DELETE From AAmzAsinToSku WHERE Id = @id AND StoreId = "+storeId;
              int effectedRows = _db.Execute(sql, new{id});
              return effectedRows >0 ;
             }
            catch (Exception ex)
            {
                return false;
            }
        }
public List<GraphData> graphPerProduct(int id, string marketplace, int storeId)
{
            //stack chart
            List<GraphData> graphData = new List<GraphData>();
            AAmzAsinToSku aAmzasinToSku = GetById(id,storeId);
                if(aAmzasinToSku == null)
                  return null;
           return null;//            graphData =  _amazonOrdersRepository.GetGraphData("US",asinToSku.Asin);
}
    }
}
