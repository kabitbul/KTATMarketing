using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class ArrivingFromChinaRepository : Repository<ArrivingFromChina> , IArrivingFromChinaRepository
    {
        private readonly ApplicationDbContext _db;
        public ArrivingFromChinaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(ArrivingFromChina arrivingFromChina)
        {
            var objFromDb = _db.arrivingFromChinas.FirstOrDefault(s=>s.Id == arrivingFromChina.Id);
            if (objFromDb != null)
            {
                objFromDb.ProductId = arrivingFromChina.ProductId;
                objFromDb.NumOfBoxes = arrivingFromChina.NumOfBoxes;
                objFromDb.Quantity = arrivingFromChina.Quantity;
                objFromDb.DateArriving = arrivingFromChina.DateArriving;
                objFromDb.Comments = arrivingFromChina.Comments; 
                objFromDb.UpdatedByAdmin = arrivingFromChina.UpdatedByAdmin;
            }
        }
    }
}
