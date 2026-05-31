using KTSite.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace KTSite.Models
{
    public class GraphDataDaily
    {

        public DateTime day { get; set; }
        public int sold { get; set; }
    }
}

