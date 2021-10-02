using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class OrderArchiveRepository : Repository<OrderArchive> , IOrderArchiveRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderArchiveRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
