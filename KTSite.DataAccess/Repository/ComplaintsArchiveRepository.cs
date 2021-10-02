using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class ComplaintsArchiveRepository : Repository<ComplaintsArchive> , IComplaintsArchiveRepository
    {
        private readonly ApplicationDbContext _db;
        public ComplaintsArchiveRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
