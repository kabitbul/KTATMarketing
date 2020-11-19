using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IArrivingFromChinaRepository : IRepository<ArrivingFromChina>
    {
        void update(ArrivingFromChina arrivingFromChina);
        ArrivingFromChina Get(long id);
    }
}
