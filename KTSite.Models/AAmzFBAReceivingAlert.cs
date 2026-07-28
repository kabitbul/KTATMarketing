using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KTSite.Models
{
public class AAmzFBAReceivingAlert
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    [MaxLength(3)]
    public string Marketplace { get; set; } = string.Empty;
    [MaxLength(20)]
    public string Asin { get; set; } = string.Empty;
    public int AvailableQty { get; set; }
    public int InboundShippedQty { get; set; }
    public int InboundReceivingQty { get; set; }
    public int ReservedQty { get; set; }
    [MaxLength(50)]
    public string? DetectionReason { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsHandled { get; set; }
    public DateTime? HandledDate { get; set; }
}
}