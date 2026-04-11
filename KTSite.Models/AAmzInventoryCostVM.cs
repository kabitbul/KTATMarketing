using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace KTSite.Models
{
    public class AAmzInventoryCostVM
    {
        public List<AAmzInventoryCost> LatestItems { get; set; } = new List<AAmzInventoryCost>();
        public List<AAmzInventoryCost> USHistory { get; set; } = new List<AAmzInventoryCost>();
        public List<AAmzInventoryCost> CAHistory { get; set; } = new List<AAmzInventoryCost>();
    }
}
