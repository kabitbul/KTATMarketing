using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KTSite.Models
{
    public class ExcelUploadsForShops
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int StoreId { get; set; }
        public long FromOrderId { get; set; }
        public long ToOrderId { get; set; }
        [DefaultValue(false)]
        public bool TrackingUpdated { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DefaultValue("Now")]
        public DateTime CreatedDate { get; set; }
    }
}
