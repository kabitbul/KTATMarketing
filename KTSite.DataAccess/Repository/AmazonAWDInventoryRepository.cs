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
    public class AmazonAWDInventoryRepository :  IAmazonAWDInventoryRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public AmazonAWDInventoryRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<AmazonInvStatistics> GetInventoryStat(string marketPlace,bool? showRestock)
        {
          return null;
         }
     }
}
