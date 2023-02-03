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
    public class BalanceUpdateRepository :  IBalanceUpdateRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public BalanceUpdateRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<BalanceUpdatesList> GetAllBalanceUpdates()
        {
        var sql =  
               "SELECT b.UserNameId, a.Name,b.Amount,b.PayDate,b.Action, b.Comments " +
               "FROM BalanceUpdates b,AspNetUsers a " +
               "WHERE b.UserNameId = a.Id Order By b.PayDate desc";
            return _db.Query<BalanceUpdatesList>(sql).ToList();
        }
public List<UserIdAndName> GetUsersList()
        {
        var sql =  
         "SELECT p.UserNameId, a.Name " +
         "FROM PaymentBalances p, AspNetUsers a " +
         "WHERE p.UserNameId = a.Id Order by a.Name";
            return _db.Query<UserIdAndName>(sql).ToList();
        }
public BalanceUpdatesList InsertBalanceUpdate(BalanceUpdatesList balanceUpdatesList)
        {
        var sql =  
         "INSERT INTO BalanceUpdates VALUES(@UserNameId,@Amount,GETDATE(),@Comments,@Action) ";
            var id = _db.Query<int>(sql, new
            {
               @UserNameId = balanceUpdatesList.UserNameId,
               @Amount = balanceUpdatesList.Amount,
               @Comments = balanceUpdatesList.Comments,
               @Action = balanceUpdatesList.Action
            }).FirstOrDefault();
         return balanceUpdatesList;
        }
public int UpdateUserBalance(double Amount, string UserNameId)
        {
            var sql =
                "UPDATE PaymentBalances "+
                 "SET Balance = Balance + @Amount "+
                 "WHERE UserNameId = @UserNameId";
            try
            {
                return _db.Execute(sql, new { @Amount = Amount,@UserNameId = UserNameId });
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
