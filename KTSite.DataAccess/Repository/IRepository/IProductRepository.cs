using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IProductRepository :IRepository<Product>
    {
        public IEnumerable<Product> getAllProducts(int id);
        void update(Product product);
    }
}
