using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IBalanceUpdateRepository
    {
       public List<BalanceUpdatesList> GetAllBalanceUpdates();
       public List<UserIdAndName> GetUsersList();
      public BalanceUpdatesList InsertBalanceUpdate(BalanceUpdatesList balanceUpdatesList);
      public int UpdateUserBalance(double Amount, string UserNameId);
    }
}
