using Microsoft.AspNetCore.Http;

namespace KTSite.Models
{
    public class fileAndStoreIdVM
    {
        public IFormFile CSVFile { get; set; }
        public int storeId { get; set; }
    }
}
